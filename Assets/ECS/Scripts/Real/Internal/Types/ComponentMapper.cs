using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECS.Scripts.Real.Interfaces;
using ECS.Scripts.Real.Types;
using ECS.Scripts.Real.Types.Attributes;

namespace ECS.Scripts.Real.Internal.Types
{
    internal class ComponentMapper
    {
        public Dictionary<Type, IAnyEntityComponentContainer> ScanForTypes()
        {
            var componentAnymap = new Dictionary<Type, IAnyEntityComponentContainer>();
            var types = ScanForFromAssembly();
            foreach (var type in types)
            {
                var containerClass = typeof(ComponentList<>);

                var wrapperClass = typeof(Component<>);
                var wrappedType = wrapperClass.MakeGenericType(type);
                
                var containerType = containerClass.MakeGenericType(wrappedType);

                var reserveSize = type.GetCustomAttribute<ReserveInComponentArray>()?.ReserveSize;

                var container = (IAnyEntityComponentContainer)Activator.CreateInstance(containerType, reserveSize);

                componentAnymap.Add(type, container);
            }

            return componentAnymap;
        }

        private static IEnumerable<TypeInfo> ScanForFromAssembly()
        {
            return Scan(Assembly.GetExecutingAssembly());
        }

        private static IEnumerable<TypeInfo> ScanForFromAssemblyContaining<TMarker>()
        {
            return Scan(typeof(TMarker).Assembly);
        }

        private static IEnumerable<TypeInfo> Scan(params Assembly[] assemblies)
        {
            IEnumerable<TypeInfo> componentTypes = null;
            foreach (var assembly in assemblies)
            {
                // Get all concrete types implementing the IComponentECS interface
                componentTypes = assembly.DefinedTypes
                    .Where(x =>
                        typeof(IComponentData).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
            }

            return componentTypes;
        }
    }
}
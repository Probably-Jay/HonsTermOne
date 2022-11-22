using System;
using System.Collections.Generic;
using System.Reflection;
using ECS.Scripts.Real.Internal.Interfaces;
using ECS.Scripts.Real.Public;
using ECS.Scripts.Real.Public.Attributes;

namespace ECS.Scripts.Real.Internal.Types
{
    internal class ComponentMapper
    {
        public Dictionary<Type, IAnyEntityComponentContainer> ScanForTypes<TMarker>()
        {
            var types = AssemblyScanner<IComponentData>.ScanForFromAssemblyContaining<TMarker>();

            return CreateAnyMapElements(types);
        }
        
        public Dictionary<Type, IAnyEntityComponentContainer> ScanForTypes(params Type[] assemblyMarkers)
        {
            var types = AssemblyScanner<IComponentData>.ScanForFromAssembliesContaining(assemblyMarkers);

            return CreateAnyMapElements(types);
        }

        private static Dictionary<Type, IAnyEntityComponentContainer> CreateAnyMapElements(IEnumerable<TypeInfo> types)
        {
            var componentAnymap = new Dictionary<Type, IAnyEntityComponentContainer>();

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
    }
}

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
        public IReadOnlyDictionary<Type, IAnyEntityComponentContainer> CreateComponentMapping(TypeRegistry typeRegistry)
        {
            var componentTypes = typeRegistry.ComponentTypes;

            return CreateAnyMapElements(componentTypes);
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

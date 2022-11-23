using System;
using System.Collections.Generic;
using System.Reflection;
using ECS.Scripts.Real.Internal.Interfaces;
using ECS.Scripts.Real.Public;
using ECS.Scripts.Real.Public.Attributes;

namespace ECS.Scripts.Real.Internal.Types
{
    internal static class ComponentMapFactory
    {
        public static IReadOnlyDictionary<Type, IAnyComponentContainer> CreateComponentMapping(IEnumerable<TypeInfo> componentTypes)
        {
            var componentAnymap = new Dictionary<Type, IAnyComponentContainer>();

            foreach (var type in componentTypes)
            {
                var containerClass = typeof(ComponentList<>);

                var wrapperClass = typeof(Component<>);
                var wrappedType = wrapperClass.MakeGenericType(type);

                var containerType = containerClass.MakeGenericType(wrappedType);

                var reserveSize = type.GetCustomAttribute<ReserveInComponentArray>()?.ReserveSize;

                var container = (IAnyComponentContainer)Activator.CreateInstance(containerType, reserveSize);

                componentAnymap.Add(type, container);
            }

            return componentAnymap;
        }
    }
}

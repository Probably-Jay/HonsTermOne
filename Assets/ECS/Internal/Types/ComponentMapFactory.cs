using System;
using System.Collections.Generic;
using System.Reflection;
using ECS.Public.Attributes;
using ECS.Public.Classes;

namespace ECS.Internal.Types
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

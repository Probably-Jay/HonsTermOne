using System;
using System.Collections.Generic;
using System.Reflection;
using ECS.Public.Attributes;
using ECS.Public.Classes;
using JetBrains.Annotations;

namespace ECS.Internal.Types
{
    internal static class ComponentMapFactory
    {
        [NotNull]
        public static IReadOnlyDictionary<Type, IAnyComponentContainer> CreateComponentMapping([NotNull] IEnumerable<TypeInfo> componentTypes)
        {
            var componentAnymap = new Dictionary<Type, IAnyComponentContainer>();

            foreach (var type in componentTypes)
            {
                var containerClass = typeof(ComponentList<>);

                var wrapperClass = typeof(ComponentEcs<>);
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

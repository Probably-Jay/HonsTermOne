using System;
using System.Collections.Generic;
using System.Reflection;
using ECS.Scripts.Real.Public;

namespace ECS.Scripts.Real.Internal.Types
{
    internal static class SystemMapFactory
    {
        public static Dictionary<Type, IAnyComponentSystem> CreateSystemMap(IReadOnlyCollection<TypeInfo> systemTypes)
        {
            var systemCollection = new Dictionary<Type, IAnyComponentSystem>();

            foreach (var type in systemTypes)
            {
                var wrapperClass = typeof(ComponentSystem<>);
                var wrappedType = wrapperClass.MakeGenericType(type);

                var systemLogic = (ISystemLogic)Activator.CreateInstance(type);
                var system = (IAnyComponentSystem)Activator.CreateInstance(wrappedType, systemLogic);

                systemCollection.Add(type, system);
            }

            return systemCollection;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Reflection;
using ECS.Scripts.Real.Internal.Exceptions;
using ECS.Scripts.Real.Public;
using ECS.Scripts.Real.Public.Attributes;

namespace ECS.Scripts.Real.Internal.Types
{
    internal static class SystemMapFactory
    {
        public static Dictionary<Type, IAnySystem> CreateSystemMap(IReadOnlyCollection<TypeInfo> systemTypes)
        {
            var systemCollection = new Dictionary<Type, IAnySystem>();

            foreach (var type in systemTypes)
            {
                var logicType = typeof(System<>);
                var systemType = logicType.MakeGenericType(type);

                var operatingAttribute = type.GetCustomAttribute<SystemOperatesOn>()??throw new SystemDoesNotSpecifyOperatingTypesException(type);
                

                var systemLogic = (ISystemLogic)Activator.CreateInstance(type);
                var system = (IAnySystem)Activator.CreateInstance(systemType, systemLogic, operatingAttribute.ModifiesTypes);

                systemCollection.Add(type, system);
            }

            return systemCollection;
        }
    }
}
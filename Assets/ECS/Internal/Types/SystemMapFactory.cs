﻿using System;
using System.Collections.Generic;
using System.Reflection;
using ECS.Internal.Exceptions;
using ECS.Internal.Interfaces;
using ECS.Public;
using ECS.Public.Attributes;

namespace ECS.Internal.Types
{
    internal static class SystemMapFactory
    {
        public static Dictionary<Type, IAnySystem> CreateSystemMap(IReadOnlyCollection<TypeInfo> systemTypes,
            OwningComponentAnymap componentArrays)
        {
            var systemCollection = new Dictionary<Type, IAnySystem>();

            foreach (var type in systemTypes)
            {
                var logicType = typeof(System<>);
                var systemType = logicType.MakeGenericType(type);


                var systemLogic = (ISystemLogic)Activator.CreateInstance(type);


                var operatingAttribute = type.GetCustomAttribute<SystemOperatesOn>()??throw new SystemDoesNotSpecifyOperatingTypesException(type);
                var operatingTypeArrayRefs = GetOperatingTypeArrayRefs(componentArrays, operatingAttribute);
                
                var system = (IAnySystem)Activator.CreateInstance(systemType, systemLogic, operatingTypeArrayRefs);

                systemCollection.Add(type, system);
            }

            return systemCollection;
        }

        private static IComponentAnymap GetOperatingTypeArrayRefs(OwningComponentAnymap componentArrays,
            SystemOperatesOn operatingAttribute) 
            => componentArrays.GetNeededComponentArrays(operatingAttribute.ModifiesTypes);
    }
}
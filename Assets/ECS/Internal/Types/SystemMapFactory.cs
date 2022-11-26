using System;
using System.Collections.Generic;
using System.Reflection;
using ECS.Internal.Exceptions;
using ECS.Internal.Interfaces;
using ECS.Public.Attributes;
using ECS.Public.Classes;
using ECS.Public.Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace ECS.Internal.Types
{
    internal static class SystemMapFactory
    {
        [NotNull]
        public static Dictionary<Type, IAnySystem> CreateSystemMap([NotNull] IReadOnlyCollection<TypeInfo> systemTypes,
            OwningComponentAnymap componentArrays)
        {
            var systemCollection = new Dictionary<Type, IAnySystem>();

            foreach (var type in systemTypes)
            {
                try
                {
                    CreateSystemType(componentArrays, type, systemCollection);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    continue;
                }
            }

            return systemCollection;
        }

        private static void CreateSystemType(OwningComponentAnymap componentArrays, TypeInfo type, Dictionary<Type, IAnySystem> systemCollection)
        {
            var logicType = typeof(System<>);
            var systemType = logicType.MakeGenericType(type);


            var systemLogic = (ISystemLogic)Activator.CreateInstance(type);


            var operatingAttribute = type.GetCustomAttribute<SystemOperatesOn>() ??
                                     throw new SystemDoesNotSpecifyOperatingTypesException(type);
            operatingAttribute.AssertValid(type.ToString());

            var operatingTypeArrayRefs = GetOperatingTypeArrayRefs(componentArrays, operatingAttribute);
            var system = (IAnySystem)Activator.CreateInstance(systemType, systemLogic, operatingTypeArrayRefs,
                operatingAttribute);

            systemCollection.Add(type, system);
        }

        private static IComponentAnymap GetOperatingTypeArrayRefs([NotNull] OwningComponentAnymap componentArrays,
            SystemOperatesOn operatingAttribute) 
            => componentArrays.GetNeededComponentArrays(operatingAttribute);
    }
}
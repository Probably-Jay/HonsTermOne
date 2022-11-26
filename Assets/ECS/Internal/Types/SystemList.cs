using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ECS.Internal.Exceptions;
using ECS.Internal.Interfaces;
using ECS.Public.Classes;
using ECS.Public.Interfaces;

namespace ECS.Internal.Types
{
    internal class SystemList
    {
        private IReadOnlyDictionary<Type, IAnySystem> systemMap;
        public void RegisterTypes([JetBrains.Annotations.NotNull] TypeRegistry typeRegistry, OwningComponentAnymap componentArrays)
        {
            systemMap = SystemMapFactory.CreateSystemMap(typeRegistry.SystemTypes, componentArrays);
        }

        public void ModifySystem<T>([NotNull] [JetBrains.Annotations.NotNull] Action<T> action) where T : class, ISystemLogic
        {
            var system = GetSystem<T>();
            system.ModifySystem(action);
        }

        public TRet QuerySystem<T, TRet>([JetBrains.Annotations.NotNull] Func<T, TRet> action) where T : class, ISystemLogic
        {
            var system = GetSystem<T>();
            return system.QuerySystem(action);
        }

        private System<T> GetSystem<T>() where T : class, ISystemLogic
        {
            try
            {
                return (System<T>)systemMap[typeof(T)];
            }
            catch (KeyNotFoundException)
            {
                throw new MissingSystemTypeException(typeof(T));
            }     
            catch (InvalidCastException)
            {
                throw new MissingSystemTypeException(typeof(T));
            } 
            catch (NullReferenceException)
            {
                throw new MissingSystemTypeException(typeof(T));
            }
        }

        
        public void ForeachSystem([NotNull]Action<Type, IAnySystem> action)
        {
            foreach (var system in systemMap)
            {
                action(system.Key, system.Value);
            }
        }


        // public List<Type> GetOperatingTypes(Type systemType)
        // {
        //     GetSystem<>()
        // }
    }
}
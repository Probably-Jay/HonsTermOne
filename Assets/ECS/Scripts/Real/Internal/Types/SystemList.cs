using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ECS.Scripts.Real.Internal.Exceptions;
using ECS.Scripts.Real.Internal.Interfaces;
using ECS.Scripts.Real.Internal.Types;

namespace ECS.Scripts.Real.Public
{
    internal class SystemList
    {
        private IReadOnlyDictionary<Type, IAnySystem> systemMap;
        public void RegisterTypes(TypeRegistry typeRegistry, OwningComponentAnymap componentArrays)
        {
            systemMap = SystemMapFactory.CreateSystemMap(typeRegistry.SystemTypes, componentArrays);
        }

        public void ModifySystem<T>([NotNull] Action<T> action) where T : class, ISystemLogic
        {
            var system = GetSystem<T>();
            system.ModifySystem(action);
        }

        public TRet QuerySystem<T, TRet>(Func<T, TRet> action) where T : class, ISystemLogic
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
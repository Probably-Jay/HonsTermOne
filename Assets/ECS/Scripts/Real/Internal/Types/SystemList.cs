using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ECS.Scripts.Real.Internal.Exceptions;
using ECS.Scripts.Real.Internal.Types;

namespace ECS.Scripts.Real.Public
{
    internal class SystemList
    {
        private IReadOnlyDictionary<Type, IAnyComponentSystem> systemMap;
        public void RegisterTypes(TypeRegistry typeRegistry)
        {
            systemMap = SystemMapFactory.CreateSystemMap(typeRegistry.SystemTypes);
        }

        public void ModifySystem<T>([NotNull] Action<T> action) where T : class, ISystemLogic
        {
            var system = GetSystem<T>();
            action(system.SystemLogic);
        }

        public TRet QuerySystem<T, TRet>(Func<T, TRet> action) where T : class, ISystemLogic
        {
            var system = GetSystem<T>();
            return action(system.SystemLogic);
        }

        private ComponentSystem<T> GetSystem<T>() where T : class, ISystemLogic
        {
            try
            {
                return (ComponentSystem<T>)systemMap[typeof(T)];
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
    }
}
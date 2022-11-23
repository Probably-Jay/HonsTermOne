using System;
using System.Collections.Generic;
using System.Linq;
using ECS.Scripts.Real.Internal.Interfaces;
using ECS.Scripts.Real.Internal.Types;
using JetBrains.Annotations;
using UnityEngine.PlayerLoop;

namespace ECS.Scripts.Real.Public
{
    internal interface IAnySystem
    {
        IReadOnlyList<Type> ModifiesTypes { get; }
        void Update(float deltaTime, UpdatableEntity updatableEntity);
    }
    public interface ISystemLogic
    {
        void Update(float deltaTime, IUpdatableEntity entity);
    }
    

    internal class System<T> : IAnySystem where T : ISystemLogic
    {
        private readonly Dictionary<Type, IAnyComponentContainer> neededComponentArrays;

        //public ISystemLogic SystemLogicInterface => SystemLogic;
        public IReadOnlyList<Type> ModifiesTypes { get; }

        private T SystemLogic { get; }


        public System(T systemLogic, IEnumerable<Type> modifiesTypes)
        {
            SystemLogic = systemLogic;
            ModifiesTypes = modifiesTypes.ToList();
        }

        public void Update(float deltaTime, UpdatableEntity updatableEntity)
        {
            SystemLogic.Update(deltaTime, updatableEntity);
        }

        public void ModifySystem([NotNull]Action<T> action) 
            => action(SystemLogic);

        public TRet QuerySystem<TRet>(Func<T, TRet> action) 
            => action(SystemLogic);
    }
}
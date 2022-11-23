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
        IReadOnlyCollection<Type> ModifiesTypes { get; }
        void Update(float deltaTime, Entity entity);
    }


    internal class System<T> : IAnySystem where T : ISystemLogic
    {
        private readonly IComponentAnymap operatingTypeArraysMappingReference;
        public IReadOnlyCollection<Type> ModifiesTypes => operatingTypeArraysMappingReference.Types;
        private T SystemLogic { get; }


        public System(T systemLogic, IComponentAnymap operatingTypeArrayRefs)
        {
            SystemLogic = systemLogic;
            operatingTypeArraysMappingReference = operatingTypeArrayRefs;
        }
        
        public void Update(float deltaTime, Entity entity)
        {
            SystemLogic.Update(deltaTime, new UpdatableEntity(entity, operatingTypeArraysMappingReference));
        }

        public void ModifySystem([NotNull]Action<T> action) 
            => action(SystemLogic);

        public TRet QuerySystem<TRet>(Func<T, TRet> action) 
            => action(SystemLogic);
    }
}
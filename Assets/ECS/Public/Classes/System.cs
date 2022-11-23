using System;
using System.Collections.Generic;
using ECS.Internal.Types;
using ECS.Public.Interfaces;
using JetBrains.Annotations;

namespace ECS.Public.Classes
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
            SystemLogic.Update(deltaTime, new SystemEntityViewView(entity, operatingTypeArraysMappingReference));
        }

        public void ModifySystem([NotNull]Action<T> action) 
            => action(SystemLogic);

        public TRet QuerySystem<TRet>(Func<T, TRet> action) 
            => action(SystemLogic);
    }
}
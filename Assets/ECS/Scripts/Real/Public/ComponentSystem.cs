using System;
using System.Collections.Generic;
using System.Linq;
using ECS.Scripts.Real.Internal.Interfaces;

namespace ECS.Scripts.Real.Public
{
    internal interface IAnySystem
    {
        ISystemLogic SystemLogicInterface { get; }
        IReadOnlyList<Type> ModifiesTypes { get; }
    }
    public interface ISystemLogic
    {
        void Update(float deltaTime, IUpdatableEntity entity);
    }
    

    internal class System<T> : IAnySystem where T : ISystemLogic
    {
        public ISystemLogic SystemLogicInterface => SystemLogic;
        public IReadOnlyList<Type> ModifiesTypes { get; }
        public T SystemLogic { get; }


        public System(T systemLogic, IEnumerable<Type> modifiesTypes)
        {
            SystemLogic = systemLogic;
            ModifiesTypes = modifiesTypes.ToList();
        }
    }
}
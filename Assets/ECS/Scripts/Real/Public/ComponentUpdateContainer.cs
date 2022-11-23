using System;
using System.Collections.Generic;
using ECS.Scripts.Real.Internal.Interfaces;
using ECS.Scripts.Real.Internal.Types;

namespace ECS.Scripts.Real.Public
{
    public interface IComponentUpdateContainer
    {
        ref T Get<T>() where T : struct, IComponentData; 
    }
    
    public class ComponentUpdateContainer : IComponentUpdateContainer
    {
        private readonly Entity entity;
        private readonly Dictionary<Type, IAnyComponentContainer> neededComponentArrays;

        internal ComponentUpdateContainer(Entity entity, Dictionary<Type, IAnyComponentContainer> neededComponentArrays)
        {
            this.entity = entity;
            this.neededComponentArrays = neededComponentArrays;
        }

        public ref T Get<T>() where T : struct, IComponentData
        {
            return ref GetList<T>().GetFrom(entity).ComponentData;
        }

        private IComponentContainer<Component<T>> GetList<T>() where T : struct, IComponentData
        {
            return (IComponentContainer<Component<T>>)neededComponentArrays[typeof(T)];
        }
    }

  

}
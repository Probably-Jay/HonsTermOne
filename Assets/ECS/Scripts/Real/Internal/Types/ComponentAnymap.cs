using System;
using System.Collections.Generic;
using ECS.Scripts.Real.Interfaces;
using ECS.Scripts.Real.Types;

namespace ECS.Scripts.Real.Internal.Types
{
    internal class ComponentAnymap
    {
        private readonly Dictionary<Type, IAnyEntityComponentContainer> mapping;

        public ComponentAnymap()
        {
            mapping = new ComponentMapper().ScanForTypes();
        }

        public void Add<T>(Component<T> item) where T : struct, IComponentData
        {
            GetList<T>().Add(item);
        }

        public ref Component<T> Get<T>(in Entity entity) where T : struct, IComponentData
        {
            return ref GetList<T>().Get(entity);
        }
        
        private IComponentContainer<Component<T>> GetList<T>() where T : struct, IComponentData 
            => (IComponentContainer<Component<T>>)mapping[typeof(T)];
    }
}

   
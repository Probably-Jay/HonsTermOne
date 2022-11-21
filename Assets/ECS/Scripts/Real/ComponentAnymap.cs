using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ECS.Scripts.Real
{
    internal class ComponentAnymap
    {
        private readonly Dictionary<Type, IAnyEntityComponentContainer> mapping;

        public ComponentAnymap()
        {
            mapping = new ComponentMapper().ScanForTypes();
        }

        public void Add<T>(EntityComponentWrapped<T> item) where T : struct, IComponentECS
        {
            GetList<T>().Add(item);
        }

        public ref EntityComponentWrapped<T> Get<T>(in Entity entity) where T : struct, IComponentECS
        {
            return ref GetList<T>().Get(entity);
        }
        
        private IComponentContainer<EntityComponentWrapped<T>> GetList<T>() where T : struct, IComponentECS 
            => (IComponentContainer<EntityComponentWrapped<T>>)mapping[typeof(T)];
    }
}

   
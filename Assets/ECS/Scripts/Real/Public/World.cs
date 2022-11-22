using System.Collections.Generic;
using ECS.Scripts.Real.Internal.Extentions;
using ECS.Scripts.Real.Internal.Interfaces;
using ECS.Scripts.Real.Internal.Types;

namespace ECS.Scripts.Real.Public
{

    public class World
    {
        private EntityList EntityArray { get; }
        private ComponentAnymap ComponentArrays { get; } = new();

        public World()
        {
            EntityArray =  new EntityList(100);
        }
        
        public void RegisterEntityTypes<TMarker>()
        {
            ComponentArrays.Init<TMarker>();
        }

        public ICollection<System.Type> GetAllRegisteredComponentTypes() => ComponentArrays.GetAllRegisteredEntityTypes();

        public Entity CreateEntity()
        {
            return EntityArray.CreateEntity(this);
        }

        public void DestroyEntity(ref Entity entity)
        {
            if(entity.IsNullEntity()) 
                return;
            ComponentArrays.RemoveAllComponentsFrom(entity);
            EntityArray.DestroyEntity(ref entity);
        }

        internal void AddComponent<T>(in Entity entity) where T : struct, IComponentData
        {
            entity.AssertIsNotNull();
            var component = new Component<T>(new T(), entity);
            ComponentArrays.Add(component);
            
            ref var addComponent = ref GetComponent<T>(entity);
            addComponent.AssertIsNotNull();
        }

        public void RemoveComponent<T>(in Entity entity) where T : struct, IComponentData
        {
            ComponentArrays.RemoveComponentFrom<T>(entity);
        }

        internal ref Component<T> GetComponent<T>(in Entity entity) where T : struct, IComponentData
        {
            entity.AssertIsNotNull();
            ref var component = ref ComponentArrays.Get<T>(entity);
            component.AssertIsNotNull();
            return ref component;
        }


        internal bool EntityExistsWithinWorld(Entity entity)
        {
            return EntityArray.ContainsEntity(entity);
        }

        public bool EntityContainsComponent<T>(in Component<T> component) where T : struct, IComponentData
        {
            return EntityExistsWithinWorld(component.Entity) && ComponentArrays.ContainsComponent(component);
        }

        public bool EntityContainsComponent<T>(in Entity entity) where T : struct, IComponentData
        {
            return EntityExistsWithinWorld(entity) && ComponentArrays.ContainsComponent<T>(entity);
        }
    }
    

  
  
    
}
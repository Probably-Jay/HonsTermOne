using ECS.Scripts.Real.Internal.Extentions;
using ECS.Scripts.Real.Internal.Interfaces;
using ECS.Scripts.Real.Internal.Types;

namespace ECS.Scripts.Real.Public
{

    public static class World
    {
        private static EntityList EntityArray { get; } = new(100);
        private static ComponentAnymap ComponentArrays { get; } = new();

        public static void RegisterEntityTypes<TMarker>()
        {
            ComponentArrays.Init<TMarker>();
        }
        public static Entity CreateEntity()
        {
            return EntityArray.CreateEntity();
        }

        public static void DestroyEntity(ref Entity entity)
        {
            if(entity.IsNullEntity()) return;
            ComponentArrays.RemoveAllComponentsFrom(entity);
            EntityArray.DestroyEntity(ref entity);
        }

        internal static ref Component<T> AddComponent<T>(in Entity entity) where T : struct, IComponentData
        {
            entity.AssertIsNotNull();
            var component = new Component<T>(new T(), entity);
            ComponentArrays.Add(component);
            return ref GetComponent<T>(entity);
        }

        internal static ref Component<T> GetComponent<T>(in Entity entity) where T : struct, IComponentData
        {
            entity.AssertIsNotNull();
            return ref ComponentArrays.Get<T>(entity);
        }

        
    }
    

  
  
    
}
using ECS.Scripts.Real.Interfaces;
using ECS.Scripts.Real.Internal.Types;

namespace ECS.Scripts.Real.Types
{

    public static class World
    {
        private static EntityList EntityArray { get; } = new(100);
        private static ComponentAnymap ComponentArrays { get; } = new();

        public static Entity CreateEntity()
        {
            return EntityArray.CreateEntity();
        }

        public static ref Component<T> AddComponent<T>(in Entity entity) where T : struct, IComponentData
        {
            var component = new Component<T>(new T(), entity);
            ComponentArrays.Add(component);
            return ref GetComponent<T>(entity);
        }

        public static ref Component<T> GetComponent<T>(in Entity entity) where T : struct, IComponentData
        {
            return ref ComponentArrays.Get<T>(entity);
        }
    }
    

  
  
    
}
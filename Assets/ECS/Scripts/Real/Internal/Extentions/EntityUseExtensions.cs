using ECS.Scripts.Real.Internal.Interfaces;
using ECS.Scripts.Real.Public;

namespace ECS.Scripts.Real.Internal.Extentions
{
    public static class EntityUseExtensions
    {
        public static bool ExistsInWorld(this in Entity entity)
        {
            return !entity.IsNullEntity() && entity.OwningWorld.EntityExistsWithinWorld(entity);
        }
        public static ref Component<T> AddComponent<T>(this in Entity entity) where T : struct, IComponentData
        {
            return ref entity.OwningWorld.AddComponent<T>(entity);
        }  
        public static void RemoveComponent<T>(this in Entity entity) where T : struct, IComponentData
        {
            entity.OwningWorld.RemoveComponent<T>(entity);
        }
        public static ref Component<T> GetComponent<T>(this in Entity entity) where T : struct, IComponentData
        {
            return ref entity.OwningWorld.GetComponent<T>(entity);
        }   
        public static void DestroyFromWorld(this ref Entity entity)
        {
            entity.OwningWorld.DestroyEntity(ref entity);
        }
    }
    
    public static class ComponentUseExtensions
    {
        public static bool ExistsAttachedToEntity<T>(this in Component<T> component) where T : struct, IComponentData
        {
            return !component.IsNullComponent() && component.Entity.OwningWorld.EntityContainsComponent(component);
        }
    }
}
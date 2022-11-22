using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        public static void AddComponent<T>(this in Entity entity) where T : struct, IComponentData
        {
            entity.OwningWorld.AddComponent<T>(entity);
        }

        public static void AddComponents(this in Entity entity, TypeList types)
        {
            entity.OwningWorld.AddComponents(entity, types);
        }

        public static bool HasComponent<T>(this in Entity entity) where T : struct, IComponentData
        {
            return entity.OwningWorld.EntityContainsComponent<T>(entity);
        }

        public static void RemoveComponent<T>(this in Entity entity) where T : struct, IComponentData
        {
            entity.OwningWorld.RemoveComponent<T>(entity);
        }

        public static IReadOnlyCollection<Type> GetAllAttachedComponentTypes(this in Entity entity) 
        {
           return entity.OwningWorld.GetAllAttachedComponentTypes(entity);
        }

        internal static ref Component<T> GetComponent<T>(this in Entity entity) where T : struct, IComponentData
        {
            return ref entity.OwningWorld.GetComponent<T>(entity);
        }

        public static void ModifyComponentData<T> (this in Entity entity, [NotNull] Component<T>.ActionRef<T> componentDelegate) where T : struct, IComponentData
        {
            ref var component = ref entity.OwningWorld.GetComponent<T>(entity);
            componentDelegate(ref component.ComponentData);
        }

        public static TRet QueryComponent<TRet,T> (this in Entity entity, [NotNull] Component<T>.FunctionRef<TRet,Component<T>> componentDelegate) where T : struct, IComponentData
        {
            ref var component = ref entity.OwningWorld.GetComponent<T>(entity);
            return componentDelegate(ref component);
        }

        public static Component<T> ReadComponent<T> (this in Entity entity) where T : struct, IComponentData
        {
            ref var component = ref entity.OwningWorld.GetComponent<T>(entity);
            return component;
        }

        public static void WriteComponent<T>(this in Entity entity, Component<T> newData) where T : struct, IComponentData
        {
            ref var component = ref entity.OwningWorld.GetComponent<T>(entity);
            component.ComponentData = newData.ComponentData;
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
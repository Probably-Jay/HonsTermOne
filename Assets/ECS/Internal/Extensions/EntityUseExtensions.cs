using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ECS.Public.Classes;
using ECS.Public.Delegates;
using ECS.Public.Interfaces;

namespace ECS.Internal.Extensions
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

        public static void AddComponents(this in Entity entity, [JetBrains.Annotations.NotNull] TypeList types)
        {
            entity.OwningWorld.AddComponents(entity, types);
        }
        public static void AddComponents<T>(this in Entity entity) where T : TypeList, new()
        {
            entity.OwningWorld.AddComponents(entity, new T());
        }

        public static bool HasComponent<T>(this in Entity entity) where T : struct, IComponentData
        {
            return entity.OwningWorld.EntityHasComponent<T>(entity);
        }
        
        public static bool HasExactComponents(this in Entity entity, [JetBrains.Annotations.NotNull] TypeList types)
        {
            return entity.HasExactComponents(types.Types);
        }

        internal static bool HasExactComponents(this in Entity entity, IReadOnlyCollection<Type> types)
        {
            return entity.OwningWorld.ComponentArraysView.EntityHasExactComponents(entity, types);
        }

        internal static bool HasAnyComponents(this Entity entity, Type[] types)
        {
            return entity.OwningWorld.ComponentArraysView.EntityHasAnyComponents(entity, types);
        }

        public static void RemoveComponent<T>(this in Entity entity) where T : struct, IComponentData
        {
            entity.OwningWorld.RemoveComponent<T>(entity);
        }

        public static IReadOnlyCollection<Type> GetAllAttachedComponentTypes(this in Entity entity) 
        {
           return entity.OwningWorld.GetAllAttachedComponentTypes(entity);
        }

        internal static ref ComponentEcs<T> GetComponent<T>(this in Entity entity) where T : struct, IComponentData
        {
            return ref entity.OwningWorld.GetComponent<T>(entity);
        }

        public static void ModifyComponentData<T> (this in Entity entity, [NotNull] [JetBrains.Annotations.NotNull] ActionRef<T> componentDelegate) where T : struct, IComponentData
        {
            ref var component = ref entity.OwningWorld.GetComponent<T>(entity);
            componentDelegate(ref component.ComponentData);
        }

        public static TRet QueryComponent<TRet,T> (this in Entity entity, [NotNull] [JetBrains.Annotations.NotNull] FunctionRef<TRet,ComponentEcs<T>> componentDelegate) where T : struct, IComponentData
        {
            ref var component = ref entity.OwningWorld.GetComponent<T>(entity);
            return componentDelegate(ref component);
        }

        public static ComponentEcs<T> ReadComponent<T> (this in Entity entity) where T : struct, IComponentData
        {
            ref var component = ref entity.OwningWorld.GetComponent<T>(entity);
            return component;
        }

        public static void WriteComponent<T>(this in Entity entity, ComponentEcs<T> newData) where T : struct, IComponentData
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
        public static bool ExistsAttachedToEntity<T>(this in ComponentEcs<T> componentEcs) where T : struct, IComponentData
        {
            return !componentEcs.IsNullComponent() && componentEcs.Entity.OwningWorld.EntityHasComponent(componentEcs);
        }
    }
}
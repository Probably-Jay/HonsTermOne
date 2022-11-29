using System;
using System.Collections.Generic;
using ECS.Internal.Extensions;
using ECS.Internal.Factories;
using ECS.Public.Classes;
using ECS.Public.Delegates;
using ECS.Public.Interfaces;
using JetBrains.Annotations;

namespace ECS.Public.Extensions
{
    [PublicAPI]
    public static class EntityExtensions
    {

        /// <returns>If the entity is the sentinel <see cref="EntityFactory.NullEntity"/></returns>
        public static bool IsNullEntity(this Entity entity) => entity.Equals(Entity.Factory.NullEntity);

        /// <returns>If the entity is not null and still exists</returns>
        public static bool ExistsInWorld(this in Entity entity)
        {
            return !entity.IsNullEntity() && entity.OwningWorld.EntityExistsWithinWorld(entity);
        }
        
        /// <summary>
        /// Adds a component to an entity
        /// </summary>
        /// <typeparam name="T">The component to add</typeparam>
        public static void AddComponent<T>(this in Entity entity) where T : struct, IComponentData
        {
            entity.OwningWorld.AddComponent<T>(entity);
        }

        /// <summary>
        /// Adds multiple components to an entity
        /// </summary>
        /// <param name="entity">This reference</param>
        /// <param name="types">The <see cref="TypeList"/> of component types to add</param>
        public static void AddComponents(this in Entity entity, [NotNull] TypeList types)
        {
            entity.OwningWorld.AddComponents(entity, types);
        }
        
        /// <summary>
        /// Adds multiple components to an entity
        /// </summary>
        /// <typeparam name="T">The <see cref="TypeList"/> of component types to add</typeparam>
        public static void AddComponents<T>(this in Entity entity) where T : TypeList, new()
        {
            entity.OwningWorld.AddComponents(entity, new T());
        }
        
        /// <typeparam name="T">The type of component</typeparam>
        /// <returns>If an entity has contains a component of a given type</returns>
        public static bool HasComponent<T>(this in Entity entity) where T : struct, IComponentData
        {
            return entity.OwningWorld.EntityHasComponent<T>(entity);
        }
        
        /// <param name="entity">This reference</param>
        /// <param name="types">The <see cref="TypeList"/> of component types</param>
        /// <returns>If a component has exactly the components provided, and no other components</returns>
        public static bool HasExactComponents(this in Entity entity, [NotNull] TypeList types)
        {
            return entity.HasExactComponents(types.Types);
        }
        
        /// <summary>
        /// Remove a component from an entity
        /// </summary>
        /// <typeparam name="T">The type of component to remove</typeparam>
        public static void RemoveComponent<T>(this in Entity entity) where T : struct, IComponentData
        {
            entity.OwningWorld.RemoveComponent<T>(entity);
        }
        
        /// <returns>A collection of all the component types attached to this entity</returns>
        public static IReadOnlyCollection<Type> GetAllAttachedComponentTypes(this in Entity entity) 
        {
           return entity.OwningWorld.GetAllAttachedComponentTypes(entity);
        }

        /// <summary>
        /// Destroy entity and remove it from the world
        /// </summary>
        public static void DestroyFromWorld(this ref Entity entity)
        {
            entity.OwningWorld.DestroyEntity(ref entity);
        }

        /// <summary>
        /// Modify a component through a delegate
        /// </summary>
        /// <param name="entity">This parameter</param>
        /// <param name="componentDelegate">The action to apply to the component</param>
        /// <typeparam name="T">The <see cref="IComponentData"/> to be modified</typeparam>
        public static void ModifyComponentData<T> (this in Entity entity, [NotNull] ActionRef<T> componentDelegate) where T : struct, IComponentData
        {
            ref var component = ref entity.OwningWorld.GetComponent<T>(entity);
            componentDelegate(ref component.ComponentData);
        }

        /// <summary>
        /// Query a component through a delegate
        /// </summary>
        /// <param name="entity">This parameter</param>
        /// <param name="componentDelegate">The function to apply to the component</param>
        /// <typeparam name="T">The <see cref="IComponentData"/> to be queried</typeparam>
        /// <typeparam name="TRet">The return type of the query. If <c>void</c> then use <see cref="EntityExtensions.ModifyComponentData{T}"/></typeparam>
        /// <returns>The result of the query</returns>
        public static TRet QueryComponent<TRet,T> (this in Entity entity, [System.Diagnostics.CodeAnalysis.NotNull] [NotNull] FunctionRef<TRet, T> componentDelegate) where T : struct, IComponentData
        {
            ref var component = ref entity.OwningWorld.GetComponent<T>(entity);
            return componentDelegate(ref component.ComponentData);
        }
        
        /// <typeparam name="T">The type of the component to be read</typeparam>
        /// <returns>A copy of the component</returns>
        /// <remarks>This function only returns a copy of the component. 
        /// Any changes made to this copy <b>will not</b> be reflected in the actual component attached</remarks>
        public static T ReadComponent<T> (this in Entity entity) where T : struct, IComponentData
        {
            ref var component = ref entity.OwningWorld.GetComponent<T>(entity);
            return component.ComponentData;
        }

        /// <summary>
        /// Overwrite the data of an attached component 
        /// </summary>
        /// <param name="entity">This reference</param>
        /// <param name="newData">The <see cref="IComponentData"/> that will overwrite the current data</param>
        /// <typeparam name="T">The type of the component to be overwritten</typeparam>
        public static void WriteComponent<T>(this in Entity entity, T newData) where T : struct, IComponentData
        {
            ref var component = ref entity.OwningWorld.GetComponent<T>(entity);
            component.ComponentData = newData;
        }

    }

}
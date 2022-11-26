using System;
using System.Collections.Generic;
using ECS.Public.Attributes;
using ECS.Public.Classes;
using ECS.Public.Interfaces;
using JetBrains.Annotations;

namespace ECS.Internal.Extensions
{
    internal static class WorldExtensions
    {
        public static bool EntityExistsWithinWorld(this World world, Entity entity)
        {
            return world.EntityArrayView.Contains(entity);
        }
        public static bool EntityHasComponent<T>(this World world,in Entity entity) where T : struct, IComponentData
        {
            return world.EntityExistsWithinWorld(entity) && world.ComponentArraysView.ContainsComponent<T>(entity);
        } 
        public static bool EntityHasComponent<T>(this World world,in ComponentEcs<T> componentEcs) where T : struct, IComponentData
        {
            return world.EntityExistsWithinWorld(componentEcs.Entity) && world.ComponentArraysView.ContainsComponent(componentEcs);
        }
        
        internal static IReadOnlyCollection<Type> GetAllAttachedComponentTypes(this World world, Entity entity)
        {
            entity.AssertIsNotNull();
            return world.ComponentArraysView.GetTypesOfAllAttachedComponents(entity);
        }
        
      
    }
}

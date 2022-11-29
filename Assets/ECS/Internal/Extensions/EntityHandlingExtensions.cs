using System;
using System.Collections.Generic;
using ECS.Internal.Exceptions;
using ECS.Internal.Interfaces;
using ECS.Internal.Types;
using ECS.Public.Attributes;
using ECS.Public.Classes;
using ECS.Public.Extensions;
using ECS.Public.Interfaces;
using JetBrains.Annotations;
using Entity = ECS.Public.Classes.Entity;

namespace ECS.Internal.Extensions
{
    internal static class EntityHandlingExtensions
    {
        internal static void AssertIsNotNull(this Entity entity)
        {
            if (entity.IsNullEntity())
                throw new EntityNullException();
        }
        internal static void AssertIsNotNull<T>(this ComponentEcs<T> componentEcs) where T : struct, IComponentData
        {
            if (componentEcs.IsNullComponent())
                throw new EntityDoesNotContainComponentException();
        }
        
        internal static bool IsNullID(this GenerationalID entity) => entity.Equals(GenerationalID.NullID);

        internal static bool IsSupersededBy(this Entity thisEntity, Entity other)
        {
            return thisEntity.GenerationalID.IsSupersededBy(other.GenerationalID);
        }

        private static bool IsSupersededBy(this GenerationalID thisId, GenerationalID other)
        {
            if (thisId.ID != other.ID)
                throw new EntityIDMismatchException();
            return thisId.CompareTo(other) > 0;
        }
        
        internal static bool MatchesTypeRestriction(this Entity entity, [NotNull] ITypeRestriction typeRestrictions)
        {
            if (typeRestrictions.HasNoRestrictions)
                return true;
            
            if (typeRestrictions.Exactly.Length > 0)
                return entity.HasExactComponents(typeRestrictions.Exactly);

            if (!entity.HasAnyComponents(typeRestrictions.Contains))
                return false;

            return !entity.HasAnyComponents(typeRestrictions.Without);
        }
        
        internal static bool HasExactComponents(this in Entity entity, IReadOnlyCollection<Type> types)
        {
            return entity.OwningWorld.ComponentArraysView.EntityHasExactComponents(entity, types);
        }

        private static bool HasAnyComponents(this Entity entity, Type[] types)
        {
            return entity.OwningWorld.ComponentArraysView.EntityHasAnyComponents(entity, types);
        }
        
        internal static ref ComponentEcs<T> GetComponent<T>(this in Entity entity) where T : struct, IComponentData
        {
            return ref entity.OwningWorld.GetComponent<T>(entity);
        }

    }
}
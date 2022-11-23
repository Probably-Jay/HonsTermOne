﻿using ECS.Internal.Exceptions;
using ECS.Internal.Interfaces;
using ECS.Internal.Types;
using ECS.Public;
using Entity = ECS.Public.Entity;

namespace ECS.Internal.Extentions
{
    public static class EntityHandlingExtensions
    {
        public static bool IsNullEntity(this Entity entity) => entity.Equals(Entity.Factory.NullEntity);
        internal static void AssertIsNotNull(this Entity entity)
        {
            if (entity.IsNullEntity())
                throw new EntityNullException();
        }
        public static bool IsNullComponent<T>(this Component<T> component) where T :struct, IComponentData => component.Entity.Equals(Entity.Factory.NullEntity);
        internal static void AssertIsNotNull<T>(this Component<T> component) where T : struct, IComponentData
        {
            if (component.IsNullComponent())
                throw new EntityDoesNotContainComponentException();
        }
        
        internal static bool IsNullID(this GenerationalID entity) => entity.Equals(GenerationalID.NullID);

        internal static bool IsSupersededBy(this Entity thisEntity, Entity other)
        {
            return thisEntity.GenerationalID.IsSupersededBy(other.GenerationalID);
        }

        internal static bool IsSupersededBy(this GenerationalID thisId, GenerationalID other)
        {
            if (thisId.ID != other.ID)
                throw new EntityIDMismatchException();
            return thisId.CompareTo(other) > 0;
        }
    }
}
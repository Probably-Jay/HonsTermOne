using ECS.Scripts.Real.Internal.Exceptions;
using ECS.Scripts.Real.Internal.Interfaces;
using ECS.Scripts.Real.Internal.Types;
using ECS.Scripts.Real.Public;
using UnityEngine.Assertions;
using Entity = ECS.Scripts.Real.Public.Entity;

namespace ECS.Scripts.Real.Internal.Extentions
{
    public static class EntityHandlingExtensions
    {
        public static bool IsNullEntity(this Entity entity) => entity.Equals(Entity.EntityFactory.NullEntity);
        internal static void AssertIsNotNull(this Entity entity)
        {
            if (entity.IsNullEntity())
                throw new EntityNullException();
        }
        public static bool IsNullComponent<T>(this Component<T> component) where T :struct, IComponentData => component.Entity.Equals(Entity.EntityFactory.NullEntity);
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
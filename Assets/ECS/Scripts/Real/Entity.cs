using System;

namespace ECS.Scripts.Real
{
    public readonly struct Entity : IEntityECS, IEquatable<Entity>
    {
        // todo remove
        public Entity(GenerationalID generationalID)
        {
            GenerationalID = generationalID;
        }
        
        public Entity EntityID => this;
        public ulong IdIndex => GenerationalID.ID;

        public GenerationalID GenerationalID { get; } 

        public bool Equals(Entity other) => GenerationalID.Equals(other.GenerationalID);

        public override bool Equals(object obj) => obj is Entity other && Equals(other);

        public static bool operator==(Entity lhs, Entity rhs) => lhs.Equals(rhs);
        public static bool operator!=(Entity lhs, Entity rhs) => !(lhs == rhs);

        public override int GetHashCode() => GenerationalID.GetHashCode();
        internal static Entity NullEntity => new Entity();
        
    }
    public static class EntityExtensions
    {
        public static bool IsNullEntity(this Entity entity) => entity.Equals(Entity.NullEntity);
        public static bool IsNullComponent(this IComponentECS component) => component.EntityID.Equals(Entity.NullEntity);
        public static bool IsNullID(this GenerationalID entity) => entity.Equals(GenerationalID.NullID);

        public static bool IsSupersededBy(this Entity thisEntity, Entity other)
        {
            return thisEntity.GenerationalID.IsSupersededBy(other.GenerationalID);
        }
        
        public static bool IsSupersededBy(this GenerationalID thisId, GenerationalID other)
        {
            if (thisId.ID != other.ID)
                throw new EntityIDMismatchException();
            return thisId.CompareTo(other) > 0;
        }
    }

    
}
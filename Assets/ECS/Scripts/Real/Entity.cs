using System;

namespace ECS.Scripts.Real
{
    public readonly struct Entity : IEntityECS, IEquatable<Entity>
    {
        private Entity(GenerationalID id)
        {
            GenerationalID = id;
        }

        public GenerationalID GenerationalID { get; } 
        
        public Entity EntityID => this;
        public ulong IdIndex => GenerationalID.ID;


        public bool Equals(Entity other) => GenerationalID.Equals(other.GenerationalID);

        public override bool Equals(object obj) => obj is Entity other && Equals(other);

        public static bool operator==(Entity lhs, Entity rhs) => lhs.Equals(rhs);
        public static bool operator!=(Entity lhs, Entity rhs) => !(lhs == rhs);

        public override int GetHashCode() => GenerationalID.GetHashCode();
        internal static Entity NullEntity => new Entity();

        public static Entity New(ulong index)
        {
            return new Entity(Real.GenerationalID.NewID(index));
        }

        public static void Reuse(ulong index, ref Entity entity)
        {
            if (!entity.IsNullEntity())
                throw new EntityMustBeDestroyedBeforeIDIsReused();
            
            entity = new Entity(GenerationalID.ReuseID(index, entity.GenerationalID));
        }

        public static void Destroy(ref Entity entity)
        {
            entity = new Entity(GenerationalID.SetIDNull(entity.GenerationalID));
        }
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
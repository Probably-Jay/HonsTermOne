using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ECS.Scripts.Real
{

    public readonly struct GenerationalID : IEquatable<GenerationalID>, IComparable<GenerationalID>
    {
        public GenerationalID(ulong id, ulong generation)
        {
            if(id == 0)
                throw new NullIDCannotBeUsedException();

            ID = id;
            Generation = generation;
        }      
        
        public GenerationalID(ulong generation)
        {
            ID = 0;
            Generation = generation;
        }

        public ulong ID { get; }
        private ulong Generation { get; }

        public static GenerationalID NullID => new GenerationalID();


        public bool Equals(GenerationalID other)
        {
            if (IsNull() && other.IsNull()) // null id is generation independent
                return true;
            return ID == other.ID && Generation == other.Generation;
        }

        private bool IsNull() => ID == 0;

        public bool Equals(GenerationalID? other) => Equals(other ?? NullID);

        public override bool Equals(object obj) => obj is GenerationalID other && Equals(other);

        public static bool operator==(GenerationalID lhs, GenerationalID rhs) => lhs.Equals(rhs);

        public static bool operator!=(GenerationalID lhs, GenerationalID rhs) => !(lhs == rhs);
        public override int GetHashCode() => HashCode.Combine(ID, Generation);


        public int CompareTo(GenerationalID other)
        {
            if (ID != other.ID)
                return ID.CompareTo(ID);
            
            if (this.IsNullID())
                throw new EntityNullException();
            
            return Generation.CompareTo(other.Generation);
        }

        public static GenerationalID NewID(ulong index)
        {
            return new GenerationalID(index, 0);
        }

        public static GenerationalID ReuseID(ulong index, GenerationalID entityGenerationalID)
        {
            return new GenerationalID(index, entityGenerationalID.Generation + 1);
        }

        public static GenerationalID SetIDNull(GenerationalID entityGenerationalID)
        {
            return new GenerationalID(generation: entityGenerationalID.Generation);
        }
        
        public override string ToString()
        {
            return $"(ID: {ID.ToString()}, Gen: {Generation.ToString()})";
        }
    }

    public class NullIDCannotBeUsedException : Exception
    {
        public NullIDCannotBeUsedException() : base("The entity id (0) cannot be used as it is a sentinel value for uninitialised components.")
        { }
    } 
    public class EntityNullException : Exception
    {
        public EntityNullException() : base("The entity id associated with this object is null.")
        { }
    }
    public class EntityIDMismatchException : Exception
    {
        public EntityIDMismatchException() : base("This operation is only valid on different generations of the same ID.")
        { }
    }  
    
    public class EntityMustBeDestroyedBeforeIDIsReused : Exception
    {
        public EntityMustBeDestroyedBeforeIDIsReused() : base("Entity can only be created when the existing entity with this ID is destroyed")
        { }
    }
}
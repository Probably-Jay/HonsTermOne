using System;
using ECS.Internal.Exceptions;
using ECS.Internal.Extensions;
using JetBrains.Annotations;

namespace ECS.Internal.Types
{
    internal readonly struct GenerationalID : IEquatable<GenerationalID>, IComparable<GenerationalID>
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
        
        [NotNull]
        public override string ToString()
        {
            return $"(ID: {ID.ToString()}, Gen: {Generation.ToString()})";
        }
    }

    
}
using System;
using ECS.Scripts.Real.Internal.Interfaces;
using ECS.Scripts.Real.Internal.Types;

namespace ECS.Scripts.Real.Public
{
    public readonly partial struct Entity : IEntity, IEquatable<Entity>
    {
        internal static partial class EntityFactory{}
        
        private Entity(GenerationalID id, World world)
        {
            GenerationalID = id;
            OwningWorld = world;
        }

        internal GenerationalID GenerationalID { get; }
        internal World OwningWorld { get; }
        
        public ulong EntityIDIndex => GenerationalID.ID;
      

        public bool Equals(Entity other) => GenerationalID.Equals(other.GenerationalID);
        public override bool Equals(object obj) => obj is Entity other && Equals(other);
        public static bool operator==(Entity lhs, Entity rhs) => lhs.Equals(rhs);
        public static bool operator!=(Entity lhs, Entity rhs) => !(lhs == rhs);
        public override int GetHashCode() => GenerationalID.GetHashCode();
        
        public override string ToString()
        {
            return $"(Entity: {GenerationalID.ToString()})";
        }
        
        
        internal delegate void ActionRef(ref Entity entity);
        
    }
}
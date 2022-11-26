using System;
using ECS.Internal.Factories;
using ECS.Internal.Interfaces;
using ECS.Internal.Types;
using JetBrains.Annotations;

namespace ECS.Public.Classes
{
    public readonly struct Entity : IEntity, IEquatable<Entity>
    {
        private Entity(GenerationalID id, World world)
        {
            GenerationalID = id;
            OwningWorld = world;
        }

        public ulong EntityIDIndex => GenerationalID.ID;

        public delegate void ActionRef(ref Entity entity);
        public delegate TRet ActionFunc<out TRet>(ref Entity entity);
        
        internal GenerationalID GenerationalID { get; }
        internal World OwningWorld { get; }
        internal static EntityFactory Factory { get; } = new EntityFactory ((id, world) => new Entity(id, world));
        
        [NotNull] public override string ToString() => $"(Entity: {GenerationalID.ToString()})";
        public bool Equals(Entity other) => GenerationalID.Equals(other.GenerationalID);
        public override bool Equals(object obj) => obj is Entity other && Equals(other);
        public static bool operator==(Entity lhs, Entity rhs) => lhs.Equals(rhs);
        public static bool operator!=(Entity lhs, Entity rhs) => !(lhs == rhs);
        public override int GetHashCode() => GenerationalID.GetHashCode();
    }
}
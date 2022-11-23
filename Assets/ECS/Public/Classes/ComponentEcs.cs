using System;
using ECS.Internal.Interfaces;
using ECS.Public.Interfaces;

namespace ECS.Public.Classes
{
    public struct ComponentEcs<T> : IComponent, IEquatable<ComponentEcs<T>> where T : struct, IComponentData
    {
        public delegate void ActionRef<T1>(ref T1 item);
        public delegate TRet FunctionRef<out TRet,T1>(ref T1 item);

        internal ComponentEcs(T componentDataEcs, Entity entity)
        {
            ComponentData = componentDataEcs;
            this.entity = entity;
        }

        public void Destroy()
        {
            Entity.Factory.Destroy(ref entity);
        }


        public T ComponentData;

        public Type DataType => typeof(T);

        private Entity entity;
        public readonly Entity Entity => entity;

        public ulong EntityIDIndex => Entity.EntityIDIndex;

        public override string ToString()
        {
            return $"Entity component {Entity.ToString()} component: {ComponentData.ToString()}";
        }

        
        public bool Equals(ComponentEcs<T> other) => entity.Equals(other.entity) && ComponentData.Equals(other.ComponentData);
        public override bool Equals(object obj) => obj is ComponentEcs<T> other && Equals(other);
        public static bool operator==(ComponentEcs<T> lhs, ComponentEcs<T> rhs) => lhs.Equals(rhs);
        public static bool operator!=(ComponentEcs<T> lhs, ComponentEcs<T> rhs) => !(lhs == rhs);
        public override int GetHashCode() => HashCode.Combine(entity, ComponentData);
    }
}
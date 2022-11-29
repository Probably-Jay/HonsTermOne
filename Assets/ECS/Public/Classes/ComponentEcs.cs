using System;
using ECS.Internal.Interfaces;
using ECS.Public.Interfaces;
using JetBrains.Annotations;

namespace ECS.Public.Classes
{
    /// <summary>
    /// Struct that wraps user-defined components. Contains the user-defined struct inline as well as the EntityID that the component is attached to
    /// </summary>
    /// <typeparam name="T">The user-defined component that this wrapper class contains</typeparam>
    public struct ComponentEcs<T> : IComponent, IEquatable<ComponentEcs<T>> where T : struct, IComponentData
    {
        internal ComponentEcs(T componentDataEcs, Entity entity)
        {
            ComponentData = componentDataEcs;
            this.entity = entity;
        }

        /// <summary>
        /// Remove this component and destroy it
        /// </summary>
        public void Destroy()
        {
            Entity.Factory.Destroy(ref entity);
        }

        /// <summary>
        /// The user-defined component that this wrapper class contains
        /// </summary>
        public T ComponentData;

        /// <summary>
        /// The type of <see cref="ComponentData"/>
        /// </summary>
        [NotNull] public Type DataType => typeof(T);

        /// <summary>
        /// The Entity that the component is attached to
        /// </summary>
        public readonly Entity Entity => entity;
        private Entity entity;
        
        readonly ulong IEntityComponent.EntityIDIndex => (Entity as IEntityComponent).EntityIDIndex;

        [NotNull] public override string ToString() => $"Entity component {Entity.ToString()} component: {ComponentData.ToString()}";
        public bool Equals(ComponentEcs<T> other) => entity.Equals(other.entity) && ComponentData.Equals(other.ComponentData);
        public override bool Equals(object obj) => obj is ComponentEcs<T> other && Equals(other);
        public static bool operator==(ComponentEcs<T> lhs, ComponentEcs<T> rhs) => lhs.Equals(rhs);
        public static bool operator!=(ComponentEcs<T> lhs, ComponentEcs<T> rhs) => !(lhs == rhs);
        public readonly override int GetHashCode() => HashCode.Combine(entity, ComponentData);
    }

}
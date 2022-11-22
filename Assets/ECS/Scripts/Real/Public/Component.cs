using ECS.Scripts.Real.Internal.Extentions;
using ECS.Scripts.Real.Internal.Interfaces;

namespace ECS.Scripts.Real.Public
{
    public struct Component<T> : IComponent where T : struct, IComponentData
    {
        private Entity entity;
        public readonly Entity Entity => entity;

        public void Destroy()
        {
            Entity.EntityFactory.Destroy(ref entity);
        }

        public ulong EntityIDIndex => Entity.EntityIDIndex;
        public bool Exists => !Entity.IsNullEntity();

        public T ComponentData;


        public Component(T componentDataEcs, Entity entity)
        {
            ComponentData = componentDataEcs;
            this.entity = entity;
        }

        public override string ToString()
        {
            return $"Entity component {Entity.ToString()} component: {ComponentData.ToString()}";
        }
    }
}
using ECS.Scripts.Real.Interfaces;
using ECS.Scripts.Real.Internal.Interfaces;

namespace ECS.Scripts.Real.Types
{
    public struct Component<T> : IComponent where T : struct, IComponentData
    {
        public Entity EntityID { get; }
        public T Data;

        public Component(T dataEcs, Entity entity)
        {
            Data = dataEcs;
            EntityID = entity;
        }

        public override string ToString()
        {
            return $"Entity component {EntityID.ToString()} component: {Data.ToString()}";
        }
    }
}
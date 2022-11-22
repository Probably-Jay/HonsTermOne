using ECS.Scripts.Real.Interfaces;
using ECS.Scripts.Real.Internal.Interfaces;

namespace ECS.Scripts.Real.Types
{
    public struct Component<T> : IComponent where T : struct, IComponentData
    {
        public Entity Entity { get; }
        public ulong EntityIDIndex => Entity.EntityIDIndex;
        
        public T Data;


        public Component(T dataEcs, Entity entity)
        {
            Data = dataEcs;
            Entity = entity;
        }

        public override string ToString()
        {
            return $"Entity component {Entity.ToString()} component: {Data.ToString()}";
        }
    }
}
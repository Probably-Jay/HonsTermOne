using ECS.Scripts.Real.Types;

namespace ECS.Scripts.Real.Internal.Interfaces
{
    internal interface IEntityComponent 
    {
        ulong EntityIDIndex { get; }
    }
    internal interface IEntity : IEntityComponent
    {
    }
    internal interface IComponent : IEntityComponent
    {
        Entity Entity { get; }
    }
}
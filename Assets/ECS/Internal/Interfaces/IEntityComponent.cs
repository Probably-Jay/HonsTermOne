using Entity = ECS.Public.Classes.Entity;

namespace ECS.Internal.Interfaces
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
        void Destroy();
    }
}
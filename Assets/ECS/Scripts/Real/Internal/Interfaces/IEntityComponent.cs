using ECS.Scripts.Real.Types;

namespace ECS.Scripts.Real.Internal.Interfaces
{
    internal interface IEntityComponent 
    {       
        Entity EntityID { get; }
    }
    internal interface IEntity : IEntityComponent
    {
    }
    internal interface IComponent : IEntityComponent
    {
    }
}
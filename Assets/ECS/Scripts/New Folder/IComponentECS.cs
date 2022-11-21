using System;

namespace ECS.Scripts.Real
{
    // todo rename
    public interface IEntityComponentECS 
    {
        Entity EntityID { get; }
    }

    public interface IEntityECS : IEntityComponentECS
    {
        GenerationalID GenerationalID { get; }
    }

    public interface IComponentECS
    {
    }
}
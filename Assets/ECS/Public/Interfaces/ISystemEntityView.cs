using ECS.Public.Interfaces;

namespace ECS.Public.Classes
{
    public interface ISystemEntityView
    {
        Entity Entity { get; }
        ref T GetComponent<T>() where T : struct, IComponentData; 
    }
}

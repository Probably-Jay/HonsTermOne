using ECS.Public.Attributes;
using ECS.Public.Classes;

namespace ECS.Public.Interfaces
{
    /// <summary>
    /// Custom system types must inherit from this interface, as well as define a <see cref="SystemOperatesOn"/> attribute
    /// </summary>
    public interface ISystemLogic
    {
        void Update(float deltaTime, ISystemEntityView entityView);
    }
}
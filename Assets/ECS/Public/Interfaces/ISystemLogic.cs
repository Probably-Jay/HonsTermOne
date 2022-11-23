using ECS.Public.Classes;

namespace ECS.Public.Interfaces
{
    public interface ISystemLogic
    {
        void Update(float deltaTime, ISystemEntityView entityView);
    }
}
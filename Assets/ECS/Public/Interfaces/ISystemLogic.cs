using ECS.Public.Classes;
using JetBrains.Annotations;

namespace ECS.Public.Interfaces
{
    public interface ISystemLogic
    {
        void Update(float deltaTime, ISystemEntityView entityView);
    }
}
using ECS.Public;

namespace ECS.Internal.Interfaces
{
    public interface ISystemLogic
    {
        void Update(float deltaTime, IUpdatableEntity entity);
    }
}
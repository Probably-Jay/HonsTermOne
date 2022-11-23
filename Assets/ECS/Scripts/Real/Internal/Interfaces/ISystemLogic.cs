using ECS.Scripts.Real.Public;

namespace ECS.Scripts.Real.Internal.Interfaces
{
    public interface ISystemLogic
    {
        void Update(float deltaTime, IUpdatableEntity entity);
    }
}
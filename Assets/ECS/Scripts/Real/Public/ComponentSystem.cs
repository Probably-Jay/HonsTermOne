namespace ECS.Scripts.Real.Public
{
    public interface ISystemLogic
    {
        void Update(float deltaTime);
    }

    internal interface IAnyComponentSystem
    { }
    

    internal class ComponentSystem<T> : IAnyComponentSystem where T : ISystemLogic
    {
        public T SystemLogic { get; }

        public ComponentSystem(T systemLogic)
        {
            SystemLogic = systemLogic;
        }

    }
}
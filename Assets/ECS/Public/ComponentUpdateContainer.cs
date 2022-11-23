using ECS.Internal.Interfaces;
using ECS.Internal.Types;

namespace ECS.Public
{
    public interface IUpdatableEntity
    {
        Entity Entity { get; }
        ref T GetComponent<T>() where T : struct, IComponentData; 
    }
    
    public readonly struct UpdatableEntity : IUpdatableEntity
    {
        public Entity Entity { get; }
        private readonly IComponentAnymap componentAnymapReference;
        
        internal UpdatableEntity(Entity entity, IComponentAnymap componentAnymapReference)
        {
            Entity = entity;
            this.componentAnymapReference = componentAnymapReference;
        }

        public ref T GetComponent<T>() where T : struct, IComponentData 
            => ref componentAnymapReference.GetComponent<T>(Entity).ComponentData;
    }

  

}
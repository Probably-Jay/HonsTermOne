using System;
using ECS.Internal.Exceptions;
using ECS.Internal.Extensions;
using ECS.Internal.Types;
using ECS.Public.Classes;
using JetBrains.Annotations;

namespace ECS.Internal.Factories
{
   
    internal class EntityFactory
    {
        private readonly Func<GenerationalID, World, Entity> createEntity;

        public EntityFactory([NotNull] Func<GenerationalID, World, Entity> createEntity)
        {
            this.createEntity = createEntity;
        }

        internal Entity NullEntity => new Entity();

        public Entity New(ulong index, World owningWorld)
        {
            return createEntity(GenerationalID.NewID(index), owningWorld);
        }
        
        public void Reuse(ulong index, ref Entity entity, World owningWorld)
        {
            if (!entity.IsNullEntity())
                throw new EntityMustBeDestroyedBeforeIDIsReused();

            entity = createEntity(GenerationalID.ReuseID(index, entity.GenerationalID), owningWorld);
        }

        public void Destroy(ref Entity entity)
        {
            entity = createEntity(GenerationalID.SetIDNull(entity.GenerationalID), entity.OwningWorld);
        }

    }
    
}
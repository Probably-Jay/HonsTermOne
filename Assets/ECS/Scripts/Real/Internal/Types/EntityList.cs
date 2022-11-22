using ECS.Scripts.Real.Internal.Extentions;
using Entity = ECS.Scripts.Real.Public.Entity;

namespace ECS.Scripts.Real.Internal.Types
{
    internal class EntityList
    {
        private readonly NonBoxingList<Entity> list;
        
        public EntityList(ulong? initialCapacity)
        {
            list = new NonBoxingList<Entity>(initialCapacity);
        }
        
        public Entity CreateEntity()
        {
            ulong index = 1; // skip index 0 as is sentinel value
            foreach (ref var entity in list)
            {
                if (!entity.IsNullEntity())
                {
                    ++index;
                    continue;
                }

                ReUseEntityID(ref entity, index);
                return entity;
            }

            return AddNewEntity(index);
        }

        private static void ReUseEntityID(ref Entity entity, ulong index)
        {
            Entity.EntityFactory.Reuse(index, ref entity);
        }

        private Entity AddNewEntity(ulong index)
        {
            var entity = Entity.EntityFactory.New(index);
            list.Add(entity);
            return entity;
        }

        public void DestroyEntity(ref Entity entity)
        {
            ref var actualEntity = ref GetEntity(entity);
            Entity.EntityFactory.Destroy(ref entity);
            Entity.EntityFactory.Destroy(ref actualEntity);
        }

        private ref Entity GetEntity(in Entity entity)
        {
            if (!EntityIsValid(entity)) 
                return ref NullEntityRef;
            
            return ref list[entity.EntityIDIndex];
        }
       
        private bool EntityIsValid(in Entity entity)
        {
            if (entity.IsNullEntity())
                return false;
            
            // entity at this entity index is out of array bounds
            if(list.IndexOutOfRange(entity.EntityIDIndex))
                return false;

            var existingEntity = list[entity.EntityIDIndex];

            // entity at this entity index is null
            if (existingEntity.IsNullEntity())
                return false;

            // entity at this index is newer and the passed in entity has been destroyed
            if(entity.IsSupersededBy(existingEntity))
                return false;

            return true;
        }
        
        
        private ref Entity NullEntityRef
        {
            get
            {
                list[0] = new(); // First element is guaranteed to always be nullEntity
                return ref list[0];
            }
        }
    }
}
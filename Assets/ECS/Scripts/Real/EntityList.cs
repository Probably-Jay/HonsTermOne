﻿namespace ECS.Scripts.Real
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
            Entity.Reuse(index, ref entity);
        }

        private Entity AddNewEntity(ulong index)
        {
            var entity = Entity.New(index);
            list.Add(entity);
            return entity;
        }

        public void DestroyEntity(in Entity entity)
        {
            ref var actualEntity = ref GetEntity(entity);
            Entity.Destroy(ref actualEntity);
        }

        private ref Entity GetEntity(in Entity entity)
        {
            if (!EntityIsValid(entity)) 
                return ref NullEntityRef;
            
            return ref list[entity.IdIndex];
        }
       
        private bool EntityIsValid(in Entity entity)
        {
            if (entity.IsNullEntity())
                return false;
            
            // entity at this entity index is out of array bounds
            if(list.IndexOutOfRange(entity.IdIndex))
                return false;

            var existingEntity = list[entity.IdIndex];

            // entity at this entity index is null
            if (existingEntity.EntityID.IsNullEntity())
                return false;

            // entity at this index is newer and the passed in entity has been destroyed
            if(entity.EntityID.IsSupersededBy(existingEntity))
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
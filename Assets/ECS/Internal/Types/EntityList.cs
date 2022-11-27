using System;
using System.Runtime.InteropServices;
using ECS.Internal.Extensions;
using ECS.Public.Classes;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Entity = ECS.Public.Classes.Entity;

namespace ECS.Internal.Types
{
    internal interface IEntityList
    {
        bool Contains(Entity entityComponent);
        ulong EntityCount(Entity.ActionFunc<bool> countDelegate);
    }
    
    internal class EntityList : IEntityList
    {
        private readonly NonBoxingList<Entity> list;
        public EntityList(ulong? initialCapacity)
        {
            list = new NonBoxingList<Entity>(initialCapacity);
        }

        public void ForeachExtantEntity([NotNull] Entity.ActionRef action)
        {
            foreach (ref var entity in list)
            {
                if (entity.IsNullEntity())
                    continue;
                action(ref entity);
            }
        } 
        public void ForeachExtantEntity([NotNull] Action<Entity> action)
        {
            foreach (ref var entity in list)
            {
                if (entity.IsNullEntity())
                    continue;
                action(entity);
            }
        }
        
        public ulong EntityCount([CanBeNull] Entity.ActionFunc<bool> countDelegate)
        {
            var count = 0ul;
            ForeachExtantEntity((ref Entity entity) =>
                {
                    if(countDelegate?.Invoke(ref entity) ?? true) 
                        count++;
                }
            );

            return count;
        }

        public Entity CreateEntity(World owningWorld)
        {
            ulong index = 1; // skip index 0 as is sentinel value
            foreach (ref var entity in list)
            {
                if (!entity.IsNullEntity())
                {
                    ++index;
                    continue;
                }

                ReUseEntityID(ref entity, index, owningWorld);
                return entity;
            }

            return AddNewEntity(index, owningWorld);
        }

        private static void ReUseEntityID(ref Entity entity, ulong index, World owningWorld)
        {
            Entity.Factory.Reuse(index, ref entity, owningWorld);
        }

        private Entity AddNewEntity(ulong index, World owningWorld)
        {
            var entity = Entity.Factory.New(index, owningWorld);
            list.Add(entity);
            return entity;
        }

        public void DestroyEntity(ref Entity entity)
        {
            ref var actualEntity = ref GetEntity(entity);
            Entity.Factory.Destroy(ref entity);
            Entity.Factory.Destroy(ref actualEntity);
        }
        
        

        private ref Entity GetEntity(in Entity entity)
        {
            if (!EntityIsValid(entity)) 
                return ref NullEntityRef;
            
            return ref list[entity.EntityIDIndex];
        }

        public bool ContainsEntity(Entity entity) 
            => !GetEntity(entity).IsNullEntity();

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


        public bool Contains(Entity entity)
            => ContainsEntity(entity);
    }
}
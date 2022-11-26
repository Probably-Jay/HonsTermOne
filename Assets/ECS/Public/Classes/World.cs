using System;
using System.Collections.Generic;
using ECS.Internal.Extensions;
using ECS.Internal.Types;
using ECS.Public.Attributes;
using ECS.Public.Interfaces;
using JetBrains.Annotations;

namespace ECS.Public.Classes
{
    public class World
    {
        public static TypeRegistry TypeRegistry { get; } = new();
        
        private EntityList EntityArray { get; }
        private OwningComponentAnymap ComponentArrays { get; } = new();

        internal IEntityList EntityArrayView => EntityArray;
        internal IComponentAnymap ComponentArraysView => ComponentArrays;

        private SystemList SystemList { get; } = new();

        public World()
        {
            EntityArray =  new EntityList(100);
            RegisterTypes();
            TypeRegistry.OnUpdatedTypeRegistry += RegisterTypes;
        }

        ~World()
        {
            TypeRegistry.OnUpdatedTypeRegistry -= RegisterTypes;
        }
        
        private void RegisterTypes()
        {
            ComponentArrays.RegisterTypes(TypeRegistry);
            SystemList.RegisterTypes(TypeRegistry, ComponentArrays);
        }

        public Entity CreateEntity()
        {
            return EntityArray.CreateEntity(this);
        }
        
        public Entity CreateEntityWithComponent<T>() where T : struct, IComponentData
        {
            var entity = CreateEntity();
            AddComponent<T>(entity);
            return entity;
        }    
        
        public Entity CreateEntityWithComponents<T>() where T : TypeList, new()
        {
            return CreateEntityWithComponents(new T());
        }   
        
        public Entity CreateEntityWithComponents(TypeList types)
        {
            var entity = CreateEntity();
            AddComponents(entity, types);
            return entity;
        }

        public ICollection<Entity> CreateEntities(ulong numberOfEntitiesToCreate) 
            => CreateEntitiesWithFunction(numberOfEntitiesToCreate, () => CreateEntity());
        
        public ICollection<Entity> CreateEntitiesWithComponent<T>(ulong numberOfEntitiesToCreate) where T : struct, IComponentData
            => CreateEntitiesWithFunction(numberOfEntitiesToCreate, () => CreateEntityWithComponent<T>()); 
        
        public ICollection<Entity> CreateEntitiesWithComponents<T>(ulong numberOfEntitiesToCreate) where T : TypeList, new()
            => CreateEntitiesWithFunction(numberOfEntitiesToCreate, () => CreateEntityWithComponents(new T())); 
        
        public ICollection<Entity> CreateEntitiesWithComponents(ulong numberOfEntitiesToCreate, TypeList types) 
            => CreateEntitiesWithFunction(numberOfEntitiesToCreate, () => CreateEntityWithComponents(types));


        [NotNull]
        private ICollection<Entity> CreateEntitiesWithFunction(ulong numberOfEntitiesToCreate, Func<Entity> entityCreationFunction)
        {
            List<Entity> entities = new();
            for (ulong i = 0; i < numberOfEntitiesToCreate; i++)
            {
                entities.Add(entityCreationFunction());
            }

            return entities;
        }   
        

        internal void DestroyEntity(ref Entity entity)
        {
            if(entity.IsNullEntity()) 
                return;
            ComponentArrays.RemoveAllComponentsFrom(entity);
            EntityArray.DestroyEntity(ref entity);
        }
        
        public void DestroyAllEntities()
        {
            EntityArray.ForeachExtantEntity((ref Entity entity) => DestroyEntity(ref entity));
        }

        internal void AddComponent<T>(in Entity entity) where T : struct, IComponentData
        {
            entity.AssertIsNotNull();
            var component = new ComponentEcs<T>(new T(), entity);
            ComponentArrays.Add(component);
            
            ref var addComponent = ref GetComponent<T>(entity);
            addComponent.AssertIsNotNull();
        }

        internal void AddComponents(in Entity entity, [NotNull] TypeList types)
        {
            foreach (var type in types.Types)
            {
                this.CallGenericFunctionFromType(type, nameof(AddComponent), entity);
            }
        }

        internal ref ComponentEcs<T> GetComponent<T>(in Entity entity) where T : struct, IComponentData
        {
            entity.AssertIsNotNull();
            ref var component = ref ComponentArrays.GetComponent<T>(entity);
            component.AssertIsNotNull();
            return ref component;
        }

        internal void RemoveComponent<T>(in Entity entity) where T : struct, IComponentData
            => ComponentArrays.RemoveComponentFrom<T>(entity);
        

        public void Tick(float deltaTime)
        {
            EntityArray.ForeachExtantEntity(entity => 
            { 
                SystemList.ForeachSystem((_, system) =>
                    {
                        var typeRestrictions = system.TypeRestriction;
                        
                        if (entity.MatchesTypeRestriction(typeRestrictions))
                            system.Update(deltaTime, entity);
                    }
                );
            });
        }
        
        
        public void ModifySystem<T>([NotNull] Action<T> action) where T : class, ISystemLogic 
            => SystemList.ModifySystem(action);
        
        public TRet QuerySystem<T, TRet>([NotNull] Func<T, TRet> action) where T : class, ISystemLogic 
            => SystemList.QuerySystem(action);
    }
}
using System;
using System.Collections.Generic;
using ECS.Internal.Extensions;
using ECS.Internal.Types;
using ECS.Public.Attributes;
using ECS.Public.Interfaces;
using JetBrains.Annotations;

namespace ECS.Public.Classes
{
    /// <summary>
    /// Contains all entities and controls the systems which operate on them 
    /// </summary>
    public class World
    {
        /// <summary>
        /// <inheritdoc cref="TypeRegistry"/>
        /// </summary>
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

        /// <summary>
        /// Create a new empty entity
        /// </summary>
        /// <returns>The created entity</returns>
        public Entity CreateEntity()
        {
            return EntityArray.CreateEntity(this);
        }
        
        /// <summary>
        /// Create a new entity with a component attached
        /// </summary>
        /// <typeparam name="T">The type of the component to attach</typeparam>
        /// <returns>The created entity</returns>
        public Entity CreateEntityWithComponent<T>() where T : struct, IComponentData
        {
            var entity = CreateEntity();
            AddComponent<T>(entity);
            return entity;
        }    
        
        /// <summary>
        /// Create a new entity with components attached
        /// </summary>
        /// <typeparam name="T">The <see cref="TypeList"/> of the components to attach</typeparam>
        /// <returns>The created entity</returns>
        public Entity CreateEntityWithComponents<T>() where T : TypeList, new()
        {
            return CreateEntityWithComponents(new T());
        }   
        
        /// <summary>
        /// Create a new entity with components attached
        /// </summary>
        /// <param name="types">The <see cref="TypeList"/> of the components to attach</param>
        /// <returns>The created entity</returns>
        public Entity CreateEntityWithComponents([NotNull] TypeList types)
        {
            var entity = CreateEntity();
            AddComponents(entity, types);
            return entity;
        }

        /// <summary>
        /// Create new empty entities
        /// </summary>
        /// <param name="numberOfEntitiesToCreate">The number of entities that should be created</param>
        /// <returns>The created entities</returns>
        public ICollection<Entity> CreateEntities(ulong numberOfEntitiesToCreate) 
            => CreateEntitiesWithFunction(numberOfEntitiesToCreate, () => CreateEntity());
        
        /// <summary>
        /// Create new entities with a component attached
        /// </summary>
        /// <param name="numberOfEntitiesToCreate">The number of entities that should be created</param>
        /// <typeparam name="T">The type of the component to attach</typeparam>
        /// <returns>The created entities</returns>
        public ICollection<Entity> CreateEntitiesWithComponent<T>(ulong numberOfEntitiesToCreate) where T : struct, IComponentData
            => CreateEntitiesWithFunction(numberOfEntitiesToCreate, () => CreateEntityWithComponent<T>()); 
        
        /// <summary>
        /// Create new entities with a component attached
        /// </summary>
        /// <param name="numberOfEntitiesToCreate">The number of entities that should be created</param>
        /// <typeparam name="T">The <see cref="TypeList"/> of the components to attach</typeparam>
        /// <returns>The created entities</returns>
        public ICollection<Entity> CreateEntitiesWithComponents<T>(ulong numberOfEntitiesToCreate) where T : TypeList, new()
            => CreateEntitiesWithFunction(numberOfEntitiesToCreate, () => CreateEntityWithComponents(new T())); 
        
        /// <summary>
        /// Create new entities with a component attached
        /// </summary>
        /// <param name="numberOfEntitiesToCreate">The number of entities that should be created</param>
        /// <param name="types">The <see cref="TypeList"/> of the components to attach</param>
        /// <returns>The created entities</returns>
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

        
        /// <summary>
        /// Destroy all entities in this world
        /// </summary>
        public void DestroyAllEntities()
        {
            EntityArray.ForeachExtantEntity((ref Entity entity) => DestroyEntity(ref entity));
        }

        internal void DestroyEntity(ref Entity entity)
        {
            if(entity.IsNullEntity()) 
                return;
            ComponentArrays.RemoveAllComponentsFrom(entity);
            EntityArray.DestroyEntity(ref entity);
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
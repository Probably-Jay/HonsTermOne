using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using ECS.Internal.Extensions;
using ECS.Internal.Types;
using ECS.Public.Attributes;
using ECS.Public.Interfaces;

namespace ECS.Public.Classes
{
    public class World
    {
        public static TypeRegistry TypeRegistry { get; } = new();
        private EntityList EntityArray { get; }
        private OwningComponentAnymap ComponentArrays { get; } = new();

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


        [JetBrains.Annotations.NotNull]
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

        internal void AddComponents(Entity entity, [JetBrains.Annotations.NotNull] TypeList types)
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
        {
            ComponentArrays.RemoveComponentFrom<T>(entity);
        }

        internal bool EntityExistsWithinWorld(Entity entity)
        {
            return EntityArray.ContainsEntity(entity);
        }

        internal bool EntityHasComponent<T>(in ComponentEcs<T> componentEcs) where T : struct, IComponentData
        {
            return EntityExistsWithinWorld(componentEcs.Entity) && ComponentArrays.ContainsComponent(componentEcs);
        }

        internal bool EntityHasComponent<T>(in Entity entity) where T : struct, IComponentData
        {
            return EntityExistsWithinWorld(entity) && ComponentArrays.ContainsComponent<T>(entity);
        }

        private bool MatchesTypeRestriction(Entity entity, [JetBrains.Annotations.NotNull] ITypeRestriction typeRestrictions)
        {
            if (typeRestrictions.HasNoRestrictions)
                return true;
            
            if (typeRestrictions.Exactly.Length > 0)
               return HasExactComponents(entity, typeRestrictions.Exactly);

            if (!HasAnyComponents(entity, typeRestrictions.Contains))
                return false;

            return !HasAnyComponents(entity, typeRestrictions.Without);
        }

        internal bool HasExactComponents(in Entity entity, IReadOnlyCollection<Type> types) 
            => ComponentArrays.EntityHasExactComponents(entity, types);

        private bool HasAnyComponents(Entity entity, Type[] types)
            => ComponentArrays.EntityHasAnyComponents(entity, types);
        

        internal IReadOnlyCollection<Type> GetAllAttachedComponentTypes(in Entity entity)
        {
            entity.AssertIsNotNull();
            return ComponentArrays.GetTypesOfAllAttachedComponents(entity);
        }

        public ulong EntityCount()
        {
            return EntityArray.EntityCount(null);
        }

        public ulong EntityCount([NotNull] Entity.ActionFunc<bool> countDelegate)
        {
            return EntityArray.EntityCount(countDelegate);
        }

        public void Tick(float deltaTime)
        {
            EntityArray.ForeachExtantEntity(entity => 
            { 
                SystemList.ForeachSystem((_, system) =>
                    {
                        var operationTypes = system.ModifiesTypes;
                        var typeRestrictions = system.TypeRestriction;
                        
                        if (MatchesTypeRestriction(entity, typeRestrictions))
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


    public class TypeRegistry
    {
        public IReadOnlyCollection<TypeInfo> ComponentTypes { get; private set; } = new List<TypeInfo>();
        public IReadOnlyCollection<TypeInfo> SystemTypes { get; private set; } = new List<TypeInfo>();

        internal event Action OnUpdatedTypeRegistry;

        public void RegisterTypesFromCurrentlyExecutingAssembly()
        {
            ComponentTypes = AssemblyScanner.ScanFromCurrentlyExecutingAssembly<IComponentData>().ToList();
            SystemTypes = AssemblyScanner.ScanFromCurrentlyExecutingAssembly<ISystemLogic>().ToList();
            OnUpdatedTypeRegistry?.Invoke();
        }
        public void RegisterTypesFromAssemblyContaining<TMarker>()
        {
            ComponentTypes = AssemblyScanner.ScanForFromAssemblyContaining<IComponentData,TMarker>().ToList();
            SystemTypes = AssemblyScanner.ScanForFromAssemblyContaining<ISystemLogic,TMarker>().ToList();
            OnUpdatedTypeRegistry?.Invoke();
        } 
        public void RegisterTypesFromAssembliesContaining(Type assemblyMarker, [JetBrains.Annotations.NotNull] params Type[] otherAssemblyMarkers)
        {
            var assemblyMarkers = new [] { assemblyMarker }.Concat(otherAssemblyMarkers).ToArray();
            ComponentTypes = AssemblyScanner.ScanForFromAssembliesContaining<IComponentData>(assemblyMarkers).ToList();
            SystemTypes = AssemblyScanner.ScanForFromAssembliesContaining<ISystemLogic>(assemblyMarkers).ToList();
            OnUpdatedTypeRegistry?.Invoke();
        }

        // public void UnregisterAllTypes()
        // {
        //     ComponentTypes = new List<TypeInfo>();
        //     SystemTypes = new List<TypeInfo>();
        //     OnUpdatedTypeRegistry?.Invoke();
        // }
    }
}
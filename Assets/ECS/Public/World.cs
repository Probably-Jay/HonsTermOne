using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using ECS.Internal.Exceptions;
using ECS.Internal.Extentions;
using ECS.Internal.Interfaces;
using ECS.Internal.Types;

namespace ECS.Public
{
    
    //public static class World

    public class World
    {
        public static TypeRegistry TypeRegistry { get; } = new TypeRegistry();
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
        
        public Entity CreateEntity<T>() where T : struct, IComponentData
        {
            var entity = CreateEntity();
            AddComponent<T>(entity);
            return entity;
        }   
        
        public Entity CreateEntity(TypeList types)
        {
            var entity = CreateEntity();
            AddComponents(entity, types);
            return entity;
        }

        public ICollection<Entity> CreateEntities(int numberOfEntitiesToCreate) 
            => CreateEntitiesWithFunction(numberOfEntitiesToCreate, () => CreateEntity());
        
        public ICollection<Entity> CreateEntities<T>(int numberOfEntitiesToCreate) where T : struct, IComponentData
            => CreateEntitiesWithFunction(numberOfEntitiesToCreate, () => CreateEntity<T>()); 
        
        public ICollection<Entity> CreateEntities(int numberOfEntitiesToCreate, TypeList types) 
            => CreateEntitiesWithFunction(numberOfEntitiesToCreate, () => CreateEntity(types));


        private ICollection<Entity> CreateEntitiesWithFunction(int numberOfEntitiesToCreate, Func<Entity> entityCreationFunction)
        {
            List<Entity> entities = new();
            for (var i = 0; i < numberOfEntitiesToCreate; i++)
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
            var component = new Component<T>(new T(), entity);
            ComponentArrays.Add(component);
            
            ref var addComponent = ref GetComponent<T>(entity);
            addComponent.AssertIsNotNull();
        }

        internal void AddComponents(Entity entity, TypeList types)
        {
            foreach (var type in types.Types)
            {
                CallGenericFunctionFromType(type, nameof(AddComponent), entity);
            }
        }

        private object CallGenericFunctionFromType(Type type, string function, Entity entity)
        {
            try
            {
                var method = GetType()
                    .GetMethod(function, BindingFlags.Instance | BindingFlags.NonPublic)
                    !.MakeGenericMethod(type)!;

                return method!.Invoke(this, new object[] { entity });
            }
            catch (NullReferenceException)
            {
                throw new InvalidTypeListException();
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException!;
            }
        }

        internal ref Component<T> GetComponent<T>(in Entity entity) where T : struct, IComponentData
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

        internal bool EntityHasComponent<T>(in Component<T> component) where T : struct, IComponentData
        {
            return EntityExistsWithinWorld(component.Entity) && ComponentArrays.ContainsComponent(component);
        }

        internal bool EntityHasComponent<T>(in Entity entity) where T : struct, IComponentData
        {
            return EntityExistsWithinWorld(entity) && ComponentArrays.ContainsComponent<T>(entity);
        }

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
            // SystemList.ForeachSystem((_, system) =>
            // {
            //     var operationTypes = system.ModifiesTypes;
            //
            //     EntityArray.ForeachExtantEntity((ref Entity entity) =>
            //         {
            //             if (entity.HasExactComponents(new TypeList(operationTypes.ToArray())))
            //                 system.Update(deltaTime, entity);
            //         }
            //     );
            // });
            EntityArray.ForeachExtantEntity(entity => 
            { 
                SystemList.ForeachSystem((_, system) =>
                    {
                        var operationTypes = system.ModifiesTypes;

                        if (entity.HasExactComponents(new TypeList(operationTypes.ToArray())))
                            system.Update(deltaTime, entity);
                    }
                );
            });
        }
    

       

        public void ModifySystem<T>([NotNull] Action<T> action) where T : class, ISystemLogic 
            => SystemList.ModifySystem(action);
        
        public TRet QuerySystem<T, TRet>([NotNull] Func<T, TRet> action) where T : class, ISystemLogic 
            => SystemList.QuerySystem(action);

        public bool HasExactComponents(in Entity entity, IReadOnlyCollection<Type> types)
        {
            return ComponentArrays.EntityHasExactComponents(entity, types);
        }
    }


    public class TypeRegistry
    {
        public IReadOnlyCollection<TypeInfo> ComponentTypes { get; private set; }
        public IReadOnlyCollection<TypeInfo> SystemTypes { get; private set; }

        internal event Action OnUpdatedTypeRegistry;

        public void RegisterTypesFromCurrentlyExecutingAssembly()
        {
            ComponentTypes = AssemblyScanner.ScanFromCurrentlyExecutingAssembly<IComponentData>().ToList();
            SystemTypes = AssemblyScanner.ScanFromCurrentlyExecutingAssembly<ISystemLogic>().ToList();
            OnUpdatedTypeRegistry?.Invoke();
        }
        public void RegisterTypesFromAssemblyContaining<TMarker>()
        {
            ComponentTypes = AssemblyScanner.ScanForFromAssemblyContaining<IComponentData,TMarker>().ToList();;
            SystemTypes = AssemblyScanner.ScanForFromAssemblyContaining<ISystemLogic,TMarker>().ToList();
            OnUpdatedTypeRegistry?.Invoke();
        } 
        public void RegisterTypesFromAssembliesContaining(Type assemblyMarker, params Type[] otherAssemblyMarkers)
        {
            var assemblyMarkers = new [] { assemblyMarker }.Concat(otherAssemblyMarkers).ToArray();
            ComponentTypes = AssemblyScanner.ScanForFromAssembliesContaining<IComponentData>(assemblyMarkers).ToList();;
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
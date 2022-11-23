using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECS.Scripts.Real.Internal.Extentions;
using ECS.Scripts.Real.Internal.Interfaces;
using ECS.Scripts.Real.Internal.Types;

namespace ECS.Scripts.Real.Public
{
    
    //public static class World

    public class World
    {
        public static TypeRegistry TypeRegistry { get; } = new TypeRegistry();
        private EntityList EntityArray { get; }
        private ComponentAnymap ComponentArrays { get; } = new();

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
            SystemList.RegisterTypes(TypeRegistry);
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
        
        public ICollection<Entity> CreateEntities<T>(int numberOfEntitiesToCreate, TypeList types) where T : struct, IComponentData
            => CreateEntitiesWithFunction(numberOfEntitiesToCreate, () => CreateEntity(types));
        
        
        public ICollection<Entity> CreateEntitiesWithFunction(int numberOfEntitiesToCreate, Func<Entity> entityCreationFunction)
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
            EntityArray.ForeachEntity(((ref Entity entity) => DestroyEntity(ref entity)));
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
                try
                {
                    var method = GetType()
                        .GetMethod(nameof(AddComponent), BindingFlags.Instance | BindingFlags.NonPublic)
                        !.MakeGenericMethod(type)!;

                    method!.Invoke(this, new object[] { entity });
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
        }

        internal ref Component<T> GetComponent<T>(in Entity entity) where T : struct, IComponentData
        {
            entity.AssertIsNotNull();
            ref var component = ref ComponentArrays.Get<T>(entity);
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

        internal bool EntityContainsComponent<T>(in Component<T> component) where T : struct, IComponentData
        {
            return EntityExistsWithinWorld(component.Entity) && ComponentArrays.ContainsComponent(component);
        }

        internal bool EntityContainsComponent<T>(in Entity entity) where T : struct, IComponentData
        {
            return EntityExistsWithinWorld(entity) && ComponentArrays.ContainsComponent<T>(entity);
        }

        public IReadOnlyCollection<Type> GetAllAttachedComponentTypes(in Entity entity)
        {
            entity.AssertIsNotNull();
            return ComponentArrays.GetTypesOfAllAttachedComponents(entity);
        }
        
        public ulong EntityCount()
        {
            return EntityArray.ActiveEntityCount;
        }


        
    }


    public class TypeRegistry
    {
        public IReadOnlyCollection<TypeInfo> ComponentTypes { get; private set; }
        public IReadOnlyCollection<TypeInfo> SystemTypes { get; private set; }

        internal event Action OnUpdatedTypeRegistry;

        public void RegisterTypesFromCurrentlyExecutingAssembly()
        {
            ComponentTypes = AssemblyScanner<IComponentData>.ScanFromCurrentlyExecutingAssembly().ToList();
            SystemTypes = AssemblyScanner<ISystemLogic>.ScanFromCurrentlyExecutingAssembly().ToList();
            OnUpdatedTypeRegistry?.Invoke();
        }
        public void RegisterTypesFromAssemblyContaining<TMarker>()
        {
            ComponentTypes = AssemblyScanner<IComponentData>.ScanForFromAssemblyContaining<TMarker>().ToList();;
            SystemTypes = AssemblyScanner<ISystemLogic>.ScanForFromAssemblyContaining<TMarker>().ToList();
            OnUpdatedTypeRegistry?.Invoke();
        } 
        public void RegisterTypesFromAssembliesContaining(Type assemblyMarker, params Type[] otherAssemblyMarkers)
        {
            var assemblyMarkers = new [] { assemblyMarker }.Concat(otherAssemblyMarkers).ToArray();
            ComponentTypes = AssemblyScanner<IComponentData>.ScanForFromAssembliesContaining(assemblyMarkers).ToList();;
            SystemTypes = AssemblyScanner<IComponentData>.ScanForFromAssembliesContaining(assemblyMarkers).ToList();
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
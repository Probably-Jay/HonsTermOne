using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECS.Scripts.Real.Internal.Extentions;
using ECS.Scripts.Real.Internal.Interfaces;
using ECS.Scripts.Real.Public;
using Entity = ECS.Scripts.Real.Public.Entity;

namespace ECS.Scripts.Real.Internal.Types
{
    internal class ComponentAnymap
    {
        private IReadOnlyDictionary<Type, IAnyEntityComponentContainer> mapping;
        
        public void RegisterTypes(TypeRegistry typeRegistry)
        {
            mapping = new ComponentMapper().CreateComponentMapping(typeRegistry);
        }
        
        public void Add<T>(Component<T> item) where T : struct, IComponentData
        {
            GetList<T>().Add(item);
        }

        public void RemoveComponentFrom<T>(in Entity entity) where T : struct, IComponentData
        {
            var componentContainer = GetList<T>();
            RemoveComponentFromEntity(componentContainer, entity);
        }

        private static void RemoveComponentFromEntity(IAnyEntityComponentContainer componentContainer, Entity entity) 
            => componentContainer.RemoveFrom(entity);

        public ref Component<T> Get<T>(in Entity entity) where T : struct, IComponentData
        {
            return ref GetList<T>().GetFrom(entity);
        }

        public bool ContainsComponent<T>(in Component<T> component) where T : struct, IComponentData
        {
            return ContainsComponent<T>(component.Entity);
        }

        public bool ContainsComponent<T>(in Entity entity) where T : struct, IComponentData
        {
            return !Get<T>(entity).IsNullComponent();
        }

        private IComponentContainer<Component<T>> GetList<T>() where T : struct, IComponentData
        {
            try
            {
                return (IComponentContainer<Component<T>>)mapping[typeof(T)];
            }
            catch (KeyNotFoundException)
            {
                throw new MissingComponentTypeException<T>();
            } 
            catch (NullReferenceException)
            {
                throw new MissingComponentTypeException<T>();
            }
        }

        public void RemoveAllComponentsFrom(in Entity entity)
        {
            foreach (var (_, componentContainer) in mapping)
            {
                RemoveComponentFromEntity(componentContainer, entity);
            }
        }

        public IEnumerable<Type> GetAllRegisteredEntityTypes() => mapping.Keys;

       
    }

    

    internal class MissingComponentTypeException<T> : Exception
    {
        public MissingComponentTypeException() : base($"Type {typeof(T)} was not found during registration. " +
                                                      $"Please ensure all types used exist within assemblies passed into {nameof(TypeRegistry)}.{nameof(World.TypeRegistry.RegisterTypesFromAssembliesContaining)}().")
        { }
    }
}

   
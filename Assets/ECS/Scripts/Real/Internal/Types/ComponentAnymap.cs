using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECS.Scripts.Real.Internal.Exceptions;
using ECS.Scripts.Real.Internal.Extentions;
using ECS.Scripts.Real.Internal.Interfaces;
using ECS.Scripts.Real.Public;
using Entity = ECS.Scripts.Real.Public.Entity;

namespace ECS.Scripts.Real.Internal.Types
{
    internal class ComponentAnymap
    {
        private IReadOnlyDictionary<Type, IAnyComponentContainer> mapping;
        
        public void RegisterTypes(TypeRegistry typeRegistry)
        {
            mapping = ComponentMapFactory.CreateComponentMapping(typeRegistry.ComponentTypes);
        }
        
        public void Add<T>(Component<T> item) where T : struct, IComponentData
        {
            GetComponentList<T>().Add(item);
        }

        public void RemoveComponentFrom<T>(in Entity entity) where T : struct, IComponentData
        {
            var componentContainer = GetComponentList<T>();
            componentContainer.RemoveFrom(entity);
        }

        public ref Component<T> Get<T>(in Entity entity) where T : struct, IComponentData
        {
            return ref GetComponentList<T>().GetFrom(entity);
        }

        public bool ContainsComponent<T>(in Component<T> component) where T : struct, IComponentData
        {
            return ContainsComponent<T>(component.Entity);
        }

        public bool ContainsComponent<T>(in Entity entity) where T : struct, IComponentData
        {
            return GetComponentList<T>().IsValidComponentOfEntity(entity);
        }

        public bool ContainsComponent(in Entity entity, Type type)
        {
            return GetComponentList(type).IsValidComponentOfEntity(entity);
        }

        private IComponentContainer<Component<T>> GetComponentList<T>() where T : struct, IComponentData
        {
            try
            {
                return (IComponentContainer<Component<T>>)GetComponentList(typeof(T));
            }
            catch (InvalidCastException)
            {
                throw new MissingComponentTypeException(typeof(T));
            }
        }

        private IAnyComponentContainer GetComponentList(Type type) 
        {
            try
            {
                return mapping[type];
            }
            catch (KeyNotFoundException)
            {
                throw new MissingComponentTypeException(type);
            }
            catch (NullReferenceException)
            {
                throw new MissingComponentTypeException(type);
            }
        }  
        

        public void RemoveAllComponentsFrom(in Entity entity)
        {
            foreach (var (_, componentContainer) in mapping)
            {
                componentContainer.RemoveFrom(entity);
            }
        }


        public IReadOnlyCollection<Type> GetTypesOfAllAttachedComponents(in Entity entity)
        {
            List<Type> types = new();
            foreach (var (type, componentContainer) in mapping)
            {
                if(componentContainer.IsValidComponentOfEntity(entity)) 
                    types.Add(type);
            }

            return types;
        }


        public Dictionary<Type, IAnyComponentContainer> GetNeededComponentArrays(IReadOnlyList<Type> operationTypes)
        {
            return mapping
                .Where( kvp => operationTypes.Contains(kvp.Key))
                .ToDictionary(dict => dict.Key, dict => dict.Value);
        }


        public bool EntityHasExactComponents(in Entity entity, IReadOnlyCollection<Type> types)
        {
            foreach (var (type, container) in mapping)
            {
                if (types.Contains(type))
                {
                    if (!container.IsValidComponentOfEntity(entity))
                        return false;
                }
                else
                { 
                    if (container.IsValidComponentOfEntity(entity))
                        return false;
                }
            }

            return true;
        }
    }

    

  
}

   
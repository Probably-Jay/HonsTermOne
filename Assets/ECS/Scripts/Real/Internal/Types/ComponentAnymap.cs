﻿using System;
using System.Collections;
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

    interface IComponentAnymap
    {
        IReadOnlyCollection<Type> Types { get; }
       // ref Component<T> Get<T>(Entity entity) where T : struct, IComponentData;
        ref Component<T> GetComponent<T>(Entity entity) where T : struct, IComponentData;
    }

    internal class ComponentAnymapBase : IComponentAnymap
    {
        private IReadOnlyDictionary<Type, IAnyComponentContainer> mapping;
        protected IEnumerable<KeyValuePair<Type, IAnyComponentContainer>> MappingEnumerator => mapping;

        protected void SetMapping(IReadOnlyDictionary<Type, IAnyComponentContainer> setMapping)
        {
            mapping = setMapping;
        }

        public ref Component<T> GetComponent<T>(Entity entity) where T : struct, IComponentData
        {
            return ref GetComponentList<T>().GetFrom(entity);
        }
        public ref Component<T> GetComponent<T>(in Entity entity) where T : struct, IComponentData
        {
            return ref GetComponentList<T>().GetFrom(in entity);
        }
        
        protected IComponentContainer<Component<T>> GetComponentList<T>() where T : struct, IComponentData
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

        protected IAnyComponentContainer GetComponentList(Type type) 
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

        protected IComponentAnymap CreateSubsetView(IReadOnlyList<Type> typeSubset)
        {
            var mappingSubset = mapping
                .Where(kvp => typeSubset.Contains(kvp.Key))
                .ToDictionary(dict => dict.Key, dict => dict.Value);
            return new ViewingComponentAnymap(mappingSubset);
        }

        public IReadOnlyCollection<Type> Types => mapping.Keys.ToArray();
    }
        
    internal class OwningComponentAnymap : ComponentAnymapBase
    {
        
        public void RegisterTypes(TypeRegistry typeRegistry)
        {
            SetMapping(ComponentMapFactory.CreateComponentMapping(typeRegistry.ComponentTypes));
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
        

        public void RemoveAllComponentsFrom(in Entity entity)
        {
            foreach (var (_, componentContainer) in MappingEnumerator)
            {
                componentContainer.RemoveFrom(entity);
            }
        }


        public IReadOnlyCollection<Type> GetTypesOfAllAttachedComponents(in Entity entity)
        {
            List<Type> types = new();
            foreach (var (type, componentContainer) in MappingEnumerator)
            {
                if(componentContainer.IsValidComponentOfEntity(entity)) 
                    types.Add(type);
            }

            return types;
        }


        public IComponentAnymap GetNeededComponentArrays(IReadOnlyList<Type> operationTypes)
        {
            return CreateSubsetView(operationTypes);
        }


        public bool EntityHasExactComponents(in Entity entity, IReadOnlyCollection<Type> types)
        {
            foreach (var (type, container) in MappingEnumerator)
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

    internal class ViewingComponentAnymap : ComponentAnymapBase
    {
        public ViewingComponentAnymap(Dictionary<Type, IAnyComponentContainer> operationTypes)
        {
            SetMapping(operationTypes);
        }
    }
}

   
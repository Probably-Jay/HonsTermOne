using System;
using System.Collections.Generic;
using System.Linq;
using ECS.Internal.Exceptions;
using ECS.Public.Classes;
using ECS.Public.Interfaces;
using Entity = ECS.Public.Classes.Entity;

namespace ECS.Internal.Types
{

    interface IComponentAnymap
    {
        IReadOnlyCollection<Type> Types { get; }
       // ref Component<T> Get<T>(Entity entity) where T : struct, IComponentData;
        ref ComponentEcs<T> GetComponent<T>(Entity entity) where T : struct, IComponentData;
    }

    internal class ComponentAnymapBase : IComponentAnymap
    {
        private IReadOnlyDictionary<Type, IAnyComponentContainer> mapping;
        protected IEnumerable<KeyValuePair<Type, IAnyComponentContainer>> MappingEnumerator => mapping;

        protected void SetMapping(IReadOnlyDictionary<Type, IAnyComponentContainer> setMapping)
        {
            mapping = setMapping;
        }

        public ref ComponentEcs<T> GetComponent<T>(Entity entity) where T : struct, IComponentData
        {
            return ref GetComponentList<T>().GetFrom(entity);
        }
        public ref ComponentEcs<T> GetComponent<T>(in Entity entity) where T : struct, IComponentData
        {
            return ref GetComponentList<T>().GetFrom(in entity);
        }
        
        protected IComponentContainer<ComponentEcs<T>> GetComponentList<T>() where T : struct, IComponentData
        {
            try
            {
                return (IComponentContainer<ComponentEcs<T>>)GetComponentList(typeof(T));
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
        
        public void Add<T>(ComponentEcs<T> item) where T : struct, IComponentData
        {
            GetComponentList<T>().Add(item);
        }

        public void RemoveComponentFrom<T>(in Entity entity) where T : struct, IComponentData
        {
            var componentContainer = GetComponentList<T>();
            componentContainer.RemoveFrom(entity);
        }

      
        public bool ContainsComponent<T>(in ComponentEcs<T> componentEcs) where T : struct, IComponentData
        {
            return ContainsComponent<T>(componentEcs.Entity);
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

   
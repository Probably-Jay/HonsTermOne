using System;
using System.Collections.Generic;
using ECS.Scripts.Real.Interfaces;
using ECS.Scripts.Real.Types;

namespace ECS.Scripts.Real.Internal.Types
{
    internal class ComponentAnymap
    {
        private Dictionary<Type, IAnyEntityComponentContainer> mapping;

        public void Init<TMarker>()
        {
            mapping = new ComponentMapper().ScanForTypes<TMarker>();
        }

        public void Add<T>(Component<T> item) where T : struct, IComponentData
        {
            GetList<T>().Add(item);
        }

        public ref Component<T> Get<T>(in Entity entity) where T : struct, IComponentData
        {
            return ref GetList<T>().Get(entity);
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
        }
    }

    internal class MissingComponentTypeException<T> : Exception
    {
        public MissingComponentTypeException() : base($"Type {typeof(T)} was not found during registration. " +
                                                      $"Please ensure all types used exist within assemblies passed into {nameof(World)}.{nameof(World.RegisterEntityTypes)}().")
        { }
    }
}

   
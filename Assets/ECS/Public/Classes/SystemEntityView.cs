using System;
using ECS.Internal.Exceptions;
using ECS.Internal.Types;
using ECS.Public.Interfaces;

namespace ECS.Public.Classes
{
    public interface ISystemEntityView
    {
        Entity Entity { get; }
        ref T GetComponent<T>() where T : struct, IComponentData; 
    }

    internal readonly struct SystemEntityViewView : ISystemEntityView
    {
        public Entity Entity { get; }
        private readonly IComponentAnymap componentAnymapReference;
        
        internal SystemEntityViewView(Entity entity, IComponentAnymap componentAnymapReference)
        {
            Entity = entity;
            this.componentAnymapReference = componentAnymapReference;
        }

        public ref T GetComponent<T>() where T : struct, IComponentData
        {
            try
            {
                return ref componentAnymapReference.GetComponent<T>(Entity).ComponentData;
            }
            catch (MissingComponentTypeException e)
            {
                // ReSharper disable once PossibleIntendedRethrow
                throw e;
            }
        }
    }

  

}
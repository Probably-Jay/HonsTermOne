using System;
using ECS.Public.Attributes;
using ECS.Public.Classes;

namespace ECS.Internal.Exceptions
{
 
    public class CannotAddDuplicateComponentException : Exception
    {
        public CannotAddDuplicateComponentException(Type t) : base($"Entity already has component {t} attached")
        { }
    }

    internal class MissingComponentTypeException : Exception
    {
        public MissingComponentTypeException(Type t) : base(
            $"Type {t} could not be found on this call. " +
            $"If attempting to access component within system, please ensure system has been marked to operate on that type with {typeof(SystemOperatesOn)} attribute." +
            $"Please also ensure all types used exist within assemblies passed into {nameof(World.TypeRegistry.RegisterTypesFromAssembliesContaining)}.")
        { }
    }
    
}
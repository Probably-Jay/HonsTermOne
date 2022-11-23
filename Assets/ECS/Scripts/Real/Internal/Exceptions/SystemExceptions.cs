using System;
using ECS.Scripts.Real.Public;
using ECS.Scripts.Real.Public.Attributes;

namespace ECS.Scripts.Real.Internal.Exceptions
{

    internal class MissingSystemTypeException : Exception
    {
        public MissingSystemTypeException(Type t) : base($"Type {t} was not found during registration. " +
                                                         $"Please ensure all types used exist within assemblies passed into {nameof(TypeRegistry)}.{nameof(World.TypeRegistry.RegisterTypesFromAssembliesContaining)}().")
        { }
    } 
    
    internal class SystemDoesNotSpecifyOperatingTypesException : Exception
    {
        public SystemDoesNotSpecifyOperatingTypesException(Type t) : base($"System {t} must have a {typeof(SystemOperatesOn)} attribute.")
        { }
    }
    
}
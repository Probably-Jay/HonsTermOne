using System;
using ECS.Scripts.Real.Public;

namespace ECS.Scripts.Real.Internal.Exceptions
{

    internal class MissingSystemTypeException : Exception
    {
        public MissingSystemTypeException(Type t) : base($"Type {t} was not found during registration. " +
                                                         $"Please ensure all types used exist within assemblies passed into {nameof(TypeRegistry)}.{nameof(World.TypeRegistry.RegisterTypesFromAssembliesContaining)}().")
        { }
    }
    
}
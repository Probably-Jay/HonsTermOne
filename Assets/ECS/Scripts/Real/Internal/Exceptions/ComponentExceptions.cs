﻿using System;
using ECS.Scripts.Real.Public;

namespace ECS.Scripts.Real.Internal.Exceptions
{
 
    public class CannotAddDuplicateComponentException : Exception
    {
        public CannotAddDuplicateComponentException(Type t) : base($"Entity already has component {t} attached")
        { }
    }

    internal class MissingComponentTypeException : Exception
    {
        public MissingComponentTypeException(Type t) : base($"Type {t} was not found during registration. " +
                                                            $"Please ensure all types used exist within assemblies passed into {nameof(TypeRegistry)}.{nameof(World.TypeRegistry.RegisterTypesFromAssembliesContaining)}().")
        { }
    }
    
}
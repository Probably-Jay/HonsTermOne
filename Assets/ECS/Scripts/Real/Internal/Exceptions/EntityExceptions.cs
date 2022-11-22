﻿using System;

namespace ECS.Scripts.Real.Internal.Exceptions
{
    public class NullIDCannotBeUsedException : Exception
    {
        public NullIDCannotBeUsedException() : base("The entity id (0) cannot be used as it is a sentinel value for uninitialised components.")
        { }
    } 
    public class EntityNullException : Exception
    {
        public EntityNullException() : base("The entity id associated with this object is null.")
        { }
    }
    public class EntityIDMismatchException : Exception
    {
        public EntityIDMismatchException() : base("This operation is only valid on different generations of the same ID.")
        { }
    }  
    
    public class EntityMustBeDestroyedBeforeIDIsReused : Exception
    {
        public EntityMustBeDestroyedBeforeIDIsReused() : base("Entity can only be created when the existing entity with this ID is destroyed")
        { }
    }
}
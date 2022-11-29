using System;

namespace ECS.Internal.Exceptions
{
    internal class NullIDCannotBeUsedException : Exception
    {
        public NullIDCannotBeUsedException() : base("The entity id (0) cannot be used as it is a sentinel value for uninitialised components.")
        { }
    }

    internal class EntityNullException : Exception
    {
        public EntityNullException() : base("The entity id associated with this object is null.")
        { }
    } 
    internal class EntityDoesNotContainComponentException : Exception
    {
        public EntityDoesNotContainComponentException() : base("The entity does not contain this component.")
        { }
    }

    internal class EntityIDMismatchException : Exception
    {
        public EntityIDMismatchException() : base("This operation is only valid on different generations of the same ID.")
        { }
    }

    internal class EntityMustBeDestroyedBeforeIDIsReused : Exception
    {
        public EntityMustBeDestroyedBeforeIDIsReused() : base("Entity can only be created when the existing entity with this ID is destroyed")
        { }
    }

   
}
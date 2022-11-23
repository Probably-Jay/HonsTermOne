using System;
using ECS.Internal.Interfaces;

namespace ECS.Internal.Exceptions
{
    internal class InvalidTypeListException : Exception
    {
        public InvalidTypeListException() : base($"Type list provided was not valid.")
        { }
    } 
    
    internal class EmptyTypeListException : Exception
    {
        public EmptyTypeListException() : base($"Type list provided must have at least one type in it.")
        { }
    }

    public class DuplicateTypesInTypeListException : Exception
    {
        public DuplicateTypesInTypeListException() : base($"Type must only have one of each type in it.")
        { }
    }
    
    public class InvalidTypesInTypeListException : Exception
    {
        public InvalidTypesInTypeListException(Type type): base($"Type must be a struct deriving from {nameof(IComponentData)}.")
        { }
    }
}
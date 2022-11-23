using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ECS.Scripts.Real.Internal.Interfaces;

namespace ECS.Scripts.Real.Public
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
    
    public interface ITypeListBuilder
    {
        public IReadOnlyCollection<Type> Types { get; }

        public ITypeListBuilder AddType<T>() where T : struct, IComponentData;
        public TypeList Complete();
    }

    public class TypeList : ITypeListBuilder
    {
        private TypeList()
        { }

        internal TypeList(IEnumerable<Type> safeTypeCollection)
        {
            types = safeTypeCollection.ToList();
        }
        
        private readonly List<Type> types = new List<Type>();
        public IReadOnlyCollection<Type> Types => types;
        public static ITypeListBuilder Create() => new TypeList();
        
        public ITypeListBuilder AddType<T>() where T : struct, IComponentData
        {
            types.Add(typeof(T));
            return this;
        }

        public TypeList Complete()
        {
            AssertValid();
            return this;
        }

        private void AssertValid()
        {
            if (types.Count == 0)
                throw new EmptyTypeListException();
            if (types.Count != types.Distinct().Count())
                throw new DuplicateTypesInTypeListException();
        }
    }
}
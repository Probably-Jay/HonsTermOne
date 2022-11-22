using System;
using System.Collections.Generic;
using ECS.Scripts.Real.Internal.Interfaces;

namespace ECS.Scripts.Real.Public
{
    internal class InvalidTypeListException : Exception
    {
        public InvalidTypeListException() : base($"Type list provided was not valid.")
        { }
    }
    
    public interface ITypeListBuilder
    {
        public IEnumerable<Type> Types { get; }

        public ITypeListBuilder AddType<T>() where T : struct, IComponentData;
        public TypeList Complete();
    }

    public class TypeList : ITypeListBuilder
    {
        private TypeList()
        { }
        private readonly List<Type> types = new List<Type>();
        public IEnumerable<Type> Types => types;
        public static ITypeListBuilder Create() => new TypeList();
        
        public ITypeListBuilder AddType<T>() where T : struct, IComponentData
        {
            types.Add(typeof(T));
            return this;
        }

        public TypeList Complete() => this;
    }
}
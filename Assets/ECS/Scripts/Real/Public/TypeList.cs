using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ECS.Scripts.Real.Internal.Exceptions;
using ECS.Scripts.Real.Internal.Interfaces;
using ECS.Scripts.Real.Internal.Types;
using JetBrains.Annotations;

namespace ECS.Scripts.Real.Public
{
  
    
    public interface ITypeListBuilder
    {
        public IReadOnlyCollection<Type> Types { get; }

        public ITypeListBuilder AddType<T>() where T : struct, IComponentData;
        ITypeListBuilder AddType(Type type);
        public TypeList Complete();
    }

    public class TypeList : ITypeListBuilder
    {
        private TypeList()
        { }
        
        protected internal TypeList([NotNull] params Type[] safeTypeCollection)
        {
            var typeBuilder = TypeList.Create();
            typeBuilder = safeTypeCollection.Aggregate(typeBuilder, (current, type) => current.AddType(type));

            types = typeBuilder.Complete().types;
        }
        
        private readonly List<Type> types = new List<Type>();
        public IReadOnlyCollection<Type> Types => types;
        public static ITypeListBuilder Create() => new TypeList();
        
        public ITypeListBuilder AddType<T>() where T : struct, IComponentData
        {
            types.Add(typeof(T));
            return this;
        }

        public ITypeListBuilder AddType(Type type)
        {
            if (!AssemblyScanner.IsConcreteAndAssignableFrom<IComponentData>(type))
                throw new InvalidTypesInTypeListException(type);
            
            types.Add(type);
            return this;
        }

        public TypeList Complete()
        {
            AssertValid();
            return this;
        }

        private void AssertValid()
        {
            if (types.Count != types.Distinct().Count())
                throw new DuplicateTypesInTypeListException();
        }
    }

   
}
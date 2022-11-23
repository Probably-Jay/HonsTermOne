using System;
using System.Collections.Generic;
using System.Linq;
using ECS.Internal.Exceptions;
using ECS.Internal.Types;
using ECS.Public.Interfaces;
using JetBrains.Annotations;

namespace ECS.Public.Classes
{
    public interface ITypeListBuilder
    {
        public ITypeListBuilder AddType<T>() where T : struct, IComponentData;
        ITypeListBuilder AddType(Type type);
        public TypeList Complete();
    }

    public class TypeList : ITypeListBuilder
    {
        private TypeList()
        { }

        public TypeList([NotNull] params Func<Type>[] safeTypeDelegateCollection)
            : this(safeTypeDelegateCollection.Select(func => func()).ToArray())
        { }

        internal TypeList([NotNull] params Type[] safeTypeCollection)
        {
            var typeBuilder = TypeList.Create();
            foreach (var type in safeTypeCollection) 
                typeBuilder = typeBuilder.AddType(type);

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
        
        public static Type Type<T>() where T : struct, IComponentData
        {
            return typeof(T);
        }

        public ITypeListBuilder AddType(Type type)
        {
            if (!AssemblyScanner.IsConcreteAndAssignableFrom<IComponentData>(type))
                throw new InvalidTypesInTypeListException(nameof(type));
            
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
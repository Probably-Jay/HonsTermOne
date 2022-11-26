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
        public TypeList Complete();
    }

    [PublicAPI]
    public class TypeList : ITypeListBuilder
    {
        private TypeList()
        { }

        protected TypeList([NotNull] params Func<Type>[] safeTypeDelegateCollection)
            : this(safeTypeDelegateCollection.Select(func => func()).ToArray())
        { }

        private TypeList([NotNull] params Type[] safeTypeCollection)
        {
            var typeBuilder = TypeList.CreateInternal();
            foreach (var type in safeTypeCollection) 
                typeBuilder = typeBuilder.AddType(type);

            types = typeBuilder.Complete().types;
        }

        private readonly List<Type> types = new();
        public IReadOnlyCollection<Type> Types => types;
        [NotNull] public static ITypeListBuilder Create() => new TypeList();
        [NotNull] private static TypeList CreateInternal() => new TypeList();
        
        [NotNull] public ITypeListBuilder AddType<T>() where T : struct, IComponentData
        {
            types.Add(typeof(T));
            return this;
        } 
        
        [NotNull] public static Type Type<T>() where T : struct, IComponentData
        {
            return typeof(T);
        }

        [NotNull] private TypeList AddType(Type type)
        {
            if (!AssemblyScanner.IsConcreteAndAssignableFrom<IComponentData>(type))
                throw new InvalidTypesInTypeListException(nameof(type));
            
            types.Add(type);
            return this;
        }

        [NotNull] public TypeList Complete()
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
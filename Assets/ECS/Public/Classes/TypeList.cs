using System;
using System.Collections.Generic;
using System.Linq;
using ECS.Internal.Exceptions;
using ECS.Internal.Types;
using ECS.Public.Interfaces;
using JetBrains.Annotations;

namespace ECS.Public.Classes
{
    /// <summary>
    /// Step in a builder for <see cref="TypeList"/>
    /// </summary>
    public interface ITypeListBuilder 
    {
        /// <summary>
        /// Add a type to the type list
        /// </summary>
        /// <typeparam name="T">The type to add. Must be a <see cref="IComponentData"/> struct</typeparam>
        /// <returns>The next step in the builder</returns>
        public ITypeListBuilder AddType<T>() where T : struct, IComponentData;
        
        /// <summary>
        /// Completed the builder and returns a <see cref="TypeList"/>
        /// </summary>
        /// <returns>The created <see cref="TypeList"/></returns>
        public TypeList Complete();
    }

    /// <summary>
    /// Helper class to define a collection of <see cref="IComponentData"/> types.
    /// </summary>
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
            var typeBuilder = CreateInternal();
            typeBuilder = safeTypeCollection.Aggregate(typeBuilder, (current, type) => current.AddType(type));

            types = typeBuilder.Complete().types;
        }

        /// <summary>
        /// The types defined in this type collection
        /// </summary>
        public IReadOnlyCollection<Type> Types => types;
        private readonly List<Type> types = new();
        
        /// <summary>
        /// Create a new <see cref="TypeList"/> with a fluent-api style
        /// </summary>
        /// <returns></returns>
        [NotNull] public static ITypeListBuilder Create() => new TypeList();
        [NotNull] private static TypeList CreateInternal() => new TypeList();
        
        [NotNull] public ITypeListBuilder AddType<T>() where T : struct, IComponentData
        {
            types.Add(typeof(T));
            return this;
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

        /// <summary>
        /// Defines a type that is a <see cref="IComponentData"/> struct
        /// </summary>
        /// <typeparam name="T">The type being defined</typeparam>
        [NotNull] public static Type Type<T>() where T : struct, IComponentData
        {
            return typeof(T);
        }

        private void AssertValid()
        {
            if (types.Count != types.Distinct().Count())
                throw new DuplicateTypesInTypeListException();
        }
    }

   
}
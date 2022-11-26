using System;
using System.Collections.Generic;
using System.Linq;
using ECS.Internal.Exceptions;
using ECS.Internal.Types;
using ECS.Public.Interfaces;
using JetBrains.Annotations;

namespace ECS.Public.Attributes
{
    internal interface ITypeRestriction
    {
        Type[] Exactly { get; }
        Type[] Contains { get; }
        Type[] Without { get; }
        bool HasNoRestrictions { get; }
    }
    
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SystemOperatesOn : Attribute, ITypeRestriction
    {
        public Type[] Exactly { get; set; } = { };
        public Type[] Contains{ get; set; } = { };
        public Type[] Without { get; set; } = { };


        public void AssertValid(string typeName)
        {
            Validate(Exactly, typeName);
            Validate(Contains, typeName);
            Validate(Without, typeName);

            AssertIfExactComponentsDefinedThenNothingElseIs(typeName);
            AssertIfNoneComponentsDefinedThenAnyAlsoIs(typeName);
            AssertIfNoneComponentsDoNotCoverAnyComponents(typeName);
        }

        public bool HasNoRestrictions => Exactly.Length == 0 && Contains.Length == 0 && Without.Length == 0;

        private void Validate([NotNull] IReadOnlyCollection<Type> types, string typeName)
        {
            if (types.Count != types.Distinct().Count())
                throw new SystemUpdateFunctionDefinesDuplicateTypesException(typeName);
            if(!types.All(AssemblyScanner.IsConcreteAndAssignableFrom<IComponentData>))
                throw new SystemUpdateFunctionDefinesNonComponentTypes(typeName);
        }

        private void AssertIfExactComponentsDefinedThenNothingElseIs(string typeName)
        {
            if(Exactly.Length == 0)
                return;
            
            if (Contains.Length > 0 || Without.Length > 0)
                throw new SystemUpdateFunctionDefinesNonsensicalQueryRelationship(typeName);
        }

        private void AssertIfNoneComponentsDefinedThenAnyAlsoIs(string typeName)
        {
            if(Without.Length == 0)
                return;
            if (Contains.Length == 0)
                throw new SystemUpdateFunctionDefinesIncompleteQueryRelationship(typeName);
        }

        private void AssertIfNoneComponentsDoNotCoverAnyComponents(string typeName)
        {
            if (Contains.Intersect(Without).Any())
                throw new SystemUpdateFunctionDefinesNonsensicalQueryRelationship(typeName);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using ECS.Internal.Types;
using ECS.Public.Classes;
using ECS.Public.Interfaces;

namespace ECS.Public.Attributes
{
    internal interface ITypeRestriction
    {
        Type[] Exactly { get; }
        Type[] Contains { get; }
        Type[] Without { get; }
        bool HasNoRestrictions { get; }
    }
    
    
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SystemOperatesOn : Attribute, ITypeRestriction
    {
        public Type[] Exactly { get; set; } = new Type[] { };
        public Type[] Contains{ get; set; } = new Type[] { };
        public Type[] Without { get; set; } = new Type[] { };


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

        private void Validate(IReadOnlyCollection<Type> types, string typeName)
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

    internal class SystemUpdateFunctionDefinesNonComponentTypes : Exception
    {
        public SystemUpdateFunctionDefinesNonComponentTypes(string typeName) : base($"System {typeName} update function defines type not deriving from {typeof(IComponentData)}")
        { }
    }

    internal class SystemUpdateFunctionDefinesDuplicateTypesException : Exception
    {
        public SystemUpdateFunctionDefinesDuplicateTypesException(string typeName) : base($"System {typeName} update function defines type to modify multiple times")
        { }
    }
    
    internal class SystemUpdateFunctionDefinesNonsensicalQueryRelationship : Exception
    {
        public SystemUpdateFunctionDefinesNonsensicalQueryRelationship(string typeName) : base($"System {typeName} " +
            $"update function defines redundant {nameof(SystemOperatesOn.Exactly)} " +
            $"and \"{nameof(SystemOperatesOn.Contains)}\" or mutually exclusive \"{nameof(SystemOperatesOn.Contains)}\" and \"{nameof(SystemOperatesOn.Without)}\"")
        { }
    }
    
    internal class SystemUpdateFunctionDefinesIncompleteQueryRelationship : Exception
    {
        public SystemUpdateFunctionDefinesIncompleteQueryRelationship(string typeName) : base($"System {typeName} " +
            $"update function defines {nameof(SystemOperatesOn.Without)} but does not define {nameof(SystemOperatesOn.Contains)}")
        { }
    }
}
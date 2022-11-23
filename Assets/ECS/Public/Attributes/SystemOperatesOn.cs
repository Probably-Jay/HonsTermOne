using System;
using System.Linq;
using ECS.Internal.Types;
using ECS.Public.Interfaces;

namespace ECS.Public.Attributes
{
   // [AttributeUsage(AttributeTargets.Class)]
    public class SystemOperatesOn : Attribute
    {
        public readonly Type[] ModifiesTypes;

        public SystemOperatesOn(params Type[] modifiesTypes)
        {
            ModifiesTypes = modifiesTypes;
            Validate();
        }

        private void Validate()
        {
            if (ModifiesTypes.Length != ModifiesTypes.Distinct().Count())
                throw new SystemUpdateFunctionDefinesDuplicateTypesException();
            if(!ModifiesTypes.All(AssemblyScanner.IsConcreteAndAssignableFrom<IComponentData>))
                throw new SystemUpdateFunctionDefinesNonComponentTypes();
        }
    }

    internal class SystemUpdateFunctionDefinesNonComponentTypes : Exception
    {
        public SystemUpdateFunctionDefinesNonComponentTypes() : base($"System update function defines type not deriving from {typeof(IComponentData)}")
        { }
    }

    internal class SystemUpdateFunctionDefinesDuplicateTypesException : Exception
    {
        public SystemUpdateFunctionDefinesDuplicateTypesException() : base("System update function defines type to modify multiple times")
        { }
    }
}
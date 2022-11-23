using System;
using System.Linq;
using ECS.Scripts.Real.Internal.Interfaces;

namespace ECS.Scripts.Real.Public.Attributes
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
            if(!ModifiesTypes.All(t =>  typeof(IComponentData).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract))
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
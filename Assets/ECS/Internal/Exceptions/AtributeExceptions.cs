using System;
using ECS.Public.Attributes;
using ECS.Public.Interfaces;

namespace ECS.Internal.Exceptions
{
    internal class SystemUpdateFunctionDefinesNonComponentTypes : Exception
    {
        public SystemUpdateFunctionDefinesNonComponentTypes(string typeName) : base(
            $"System {typeName} cannot operate on types not deriving from {typeof(IComponentData)}")
        { }
    }

    internal class SystemUpdateFunctionDefinesDuplicateTypesException : Exception
    {
        public SystemUpdateFunctionDefinesDuplicateTypesException(string typeName) : base(
            $"System {typeName} cannot define the same type to operate on multiple times")
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECS.Scripts.Real.Internal.Types;

namespace ECS.Scripts.Real.Public
{
    internal class SystemList
    {
        private IReadOnlyCollection<TypeInfo> systemTypes;

        public void RegisterTypes(TypeRegistry typeRegistry)
        {
            systemTypes = typeRegistry.SystemTypes;
        }
    }
}
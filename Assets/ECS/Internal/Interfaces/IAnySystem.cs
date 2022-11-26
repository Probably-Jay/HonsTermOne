using System;
using System.Collections.Generic;
using ECS.Public.Attributes;
using ECS.Public.Classes;

namespace ECS.Internal.Interfaces
{
    internal interface IAnySystem
    {
        IReadOnlyCollection<Type> ModifiesTypes { get; }
        ITypeRestriction TypeRestriction { get; }
        void Update(float deltaTime, Entity entity);
    }
}

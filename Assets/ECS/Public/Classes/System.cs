using System;
using System.Collections.Generic;
using ECS.Internal.Interfaces;
using ECS.Internal.Types;
using ECS.Public.Attributes;
using ECS.Public.Interfaces;
using JetBrains.Annotations;

namespace ECS.Public.Classes
{
    /// <summary>
    /// Class that wraps user-defined systems. Contains the custom system logic and references to the component arrays which match the
    /// infra-system's <see cref="SystemOperatesOn"/> types
    /// </summary>
    /// <typeparam name="T">The user-defined system logic</typeparam>
    internal class System<T> : IAnySystem where T : ISystemLogic
    {
        private readonly IComponentAnymap operatingTypeArraysMappingReference;
        public IReadOnlyCollection<Type> ModifiesTypes => operatingTypeArraysMappingReference.Types;
        private T SystemLogic { get; }
        public ITypeRestriction TypeRestriction { get; }


        public System(T systemLogic, IComponentAnymap operatingTypeArrayRefs, ITypeRestriction typeRestrictions)
        {
            SystemLogic = systemLogic;
            TypeRestriction = typeRestrictions;
            operatingTypeArraysMappingReference = operatingTypeArrayRefs;
        }
        
        public void Update(float deltaTime, Entity entity)
        {
            SystemLogic.Update(deltaTime, new SystemEntityViewView(entity, operatingTypeArraysMappingReference));
        }

        public void ModifySystem([NotNull]Action<T> action) 
            => action(SystemLogic);

        public TRet QuerySystem<TRet>([NotNull] Func<T, TRet> action) 
            => action(SystemLogic);
    }
}
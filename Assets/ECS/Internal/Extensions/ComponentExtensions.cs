using ECS.Public.Classes;
using ECS.Public.Interfaces;
using JetBrains.Annotations;

namespace ECS.Internal.Extensions
{
    [PublicAPI]
    internal static class ComponentExtensions
    {
        public static bool IsNullComponent<T>(this ComponentEcs<T> componentEcs) where T :struct, IComponentData 
            => componentEcs.Entity.Equals(Entity.Factory.NullEntity);

        public static bool ExistsAttachedToEntity<T>(this in ComponentEcs<T> componentEcs) where T : struct, IComponentData
        {
            return !componentEcs.IsNullComponent() && componentEcs.Entity.OwningWorld.EntityHasComponent(componentEcs);
        }
    }
}

using ECS.Public.Classes;
using ECS.Public.Delegates;
using JetBrains.Annotations;

namespace ECS.Public.Extensions
{
    public static class WorldExtensions
    {
        /// <summary>
        /// The number of active entities in this world
        /// </summary>
        /// <param name="world">This parameter</param>
        /// <returns>The number of active entities in this world</returns>
        public static ulong EntityCount(this World world)
        {
            return world.EntityArrayView.EntityCount(null);
        }

        /// <summary>
        /// The number of active entities in this world that are true for a given delegate
        /// </summary>
        /// <param name="world">This parameter</param>
        /// <param name="countDelegate">If this delegate returns <c>true</c> then the entity will be counted, else it will be ignored</param>
        /// <returns>The number of active entities in this world that match the delegate</returns>
        public static ulong EntityCount(this World world, [NotNull] EntityActionFunc<bool> countDelegate)
        {
            return world.EntityArrayView.EntityCount(countDelegate);
        }
    }
}

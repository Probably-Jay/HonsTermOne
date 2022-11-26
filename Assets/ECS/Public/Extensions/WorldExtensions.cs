using ECS.Public.Classes;
using JetBrains.Annotations;

namespace ECS.Public.Extensions
{
    public static class WorldExtensions
    {
        public static ulong EntityCount(this World world)
        {
            return world.EntityArrayView.EntityCount(null);
        }

        public static ulong EntityCount(this World world, [NotNull] Entity.ActionFunc<bool> countDelegate)
        {
            return world.EntityArrayView.EntityCount(countDelegate);
        }
    }
}

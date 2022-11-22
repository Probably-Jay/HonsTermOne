using ECS.Scripts.Real.Internal.Exceptions;
using ECS.Scripts.Real.Internal.Extentions;
using ECS.Scripts.Real.Internal.Types;

// ReSharper disable once CheckNamespace
namespace ECS.Scripts.Real.Types
{
    public readonly partial struct Entity
    {
        internal static partial class EntityFactory
        {
            internal static Entity NullEntity => new Entity();

            public static Entity New(ulong index)
            {
                return new Entity(GenerationalID.NewID(index));
            }

            public static void Reuse(ulong index, ref Entity entity)
            {
                if (!entity.IsNullEntity())
                    throw new EntityMustBeDestroyedBeforeIDIsReused();

                entity = new Entity(GenerationalID.ReuseID(index, entity.GenerationalID));
            }

            public static void Destroy(ref Entity entity)
            {
                entity = new Entity(GenerationalID.SetIDNull(entity.GenerationalID));
            }
        }
    }
}
using ECS.Scripts.Real.Internal.Exceptions;
using ECS.Scripts.Real.Internal.Extentions;
using ECS.Scripts.Real.Internal.Types;

// ReSharper disable once CheckNamespace
namespace ECS.Scripts.Real.Public
{
    public readonly partial struct Entity
    {
        internal static partial class EntityFactory
        {
            internal static Entity NullEntity => new Entity();

            public static Entity New(ulong index, World owningWorld)
            {
                return new Entity(GenerationalID.NewID(index), owningWorld);
            }
            
            public static void Reuse(ulong index, ref Entity entity, World owningWorld)
            {
                if (!entity.IsNullEntity())
                    throw new EntityMustBeDestroyedBeforeIDIsReused();

                entity = new Entity(GenerationalID.ReuseID(index, entity.GenerationalID), owningWorld);
            }

            public static void Destroy(ref Entity entity)
            {
                entity = new Entity(GenerationalID.SetIDNull(entity.GenerationalID), entity.OwningWorld);
            }

        }
    }
}
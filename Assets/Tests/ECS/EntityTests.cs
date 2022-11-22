using ECS.Scripts.Real.Internal.Exceptions;
using ECS.Scripts.Real.Internal.Extentions;
using ECS.Scripts.Real.Public;
using NUnit.Framework;

namespace Tests.ECS
{
    public class EntityTests
    {
        private World world;

        [OneTimeSetUp]
        public void SetUp()
        {
            world = new World();
        }

        [Test]
        public void CreateEntity()
        {
            var entity = world.CreateEntity();
            
            Assert.IsFalse(entity.IsNullEntity());
        }
        
        [Test]
        public void CreatedEntityExists()
        {
            var entity = world.CreateEntity();
            
            Assert.IsTrue(entity.ExistsInWorld());
        }
        
        [Test]
        public void DestroyEntityDestroysSelf()
        {
            var entity = world.CreateEntity();
            
            entity.DestroyFromWorld();
            
            Assert.IsTrue(entity.IsNullEntity());
        }     
        
        [Test]
        public void DestroyEntityIsDestroyedFromWorld()
        {
            var entity = world.CreateEntity();

            var copy = entity;
            
            entity.DestroyFromWorld();
            
            Assert.False(copy.ExistsInWorld());
        }
    }
}
using ECS.Scripts.Real.Internal.Exceptions;
using ECS.Scripts.Real.Internal.Extentions;
using ECS.Scripts.Real.Public;
using NUnit.Framework;
using UnityEngine;

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
        public void CreatedEntityHasType()
        {
            var entity = world.CreateEntity<TestComponent>();
            
            Assert.IsTrue(entity.HasComponent<TestComponent>());
        } 
        
        [Test]
        public void CreatedEntityHasTypes()
        {
            var types = TypeList.Create()
                .AddType<TestComponent>()
                .AddType<OtherTestComponent>()
                .Complete();
            
            var entity = world.CreateEntity(types);

            var typesOnEntity = entity.GetAllAttachedComponentTypes();
            CollectionAssert.AreEquivalent(typesOnEntity,types.Types);
        }
        
        [Test]
        public void CreatesMultipleEntities()
        {
            var entities = world.CreateEntities(100);

            foreach (var entity in entities)
            {
                Assert.IsTrue(entity.ExistsInWorld());
            }

            Assert.AreEqual(100, world.EntityCount());
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
        
        [Test]
        public void DoubleDestroyEntityHasNoEffect()
        {
            var entity = world.CreateEntity();

            entity.DestroyFromWorld();
            entity.DestroyFromWorld();
        }

        [TearDown]
        public void Teardown()
        {
            world.DestroyAllEntities();
        }
    }
}
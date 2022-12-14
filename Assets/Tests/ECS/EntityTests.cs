using System;
using System.Linq;
using ECS.Internal.Extensions;
using ECS.Public;
using ECS.Public.Classes;
using ECS.Public.Extensions;
using NUnit.Framework;
using UnityEngine;
using Entity = ECS.Public.Classes.Entity;

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
            var entity = world.CreateEntityWithComponent<TestComponent>();
            
            Assert.IsTrue(entity.HasComponent<TestComponent>());
        } 
        
        [Test]
        public void CreatedEntityHasTypes()
        {
            var types = TypeList.Create()
                .AddType<TestComponent>()
                .AddType<OtherTestComponent>()
                .Complete();
            
            var entity = world.CreateEntityWithComponents(types);

            var typesOnEntity = entity.GetAllAttachedComponentTypes();
            CollectionAssert.AreEquivalent(typesOnEntity,types.Types);
            
            Assert.AreEqual(1, world.EntityCount((ref Entity e) => e.HasExactComponents(types)));
            
            Assert.AreEqual(0, world.EntityCount(
                (ref Entity e) => e.HasExactComponents(TypeList.Create().AddType<TestComponent>().Complete())
                ));
            
            Assert.AreEqual(0, world.EntityCount(
                (ref Entity e) => e.HasExactComponents(
                    TypeList.Create().AddType<TestComponent>().AddType<OtherTestComponent>().AddType<AnotherTestComponent>().Complete())
                ));
        } 
        
        [Test]
        public void CreatedEntityHasPreSpecifiedType()
        {
            var types = new MyType();
            
            var entity = world.CreateEntityWithComponents(types);

            var typesOnEntity = entity.GetAllAttachedComponentTypes();
            CollectionAssert.AreEquivalent(typesOnEntity,types.Types);
            
            Assert.AreEqual(1, world.EntityCount((ref Entity e) => e.HasExactComponents(types)));
            
            Assert.AreEqual(0, world.EntityCount(
                (ref Entity e) => e.HasExactComponents(TypeList.Create().AddType<TestComponent>().Complete())
                ));
            
            Assert.AreEqual(0, world.EntityCount(
                (ref Entity e) => e.HasExactComponents(
                    TypeList.Create().AddType<TestComponent>().AddType<OtherTestComponent>().AddType<AnotherTestComponent>().Complete())
                ));
        }
        
        [Test]
        public void CreatesMultipleEntities()
        {
            var numberOfEntitiesToCreate = 100ul;
            var entities = world.CreateEntities(numberOfEntitiesToCreate);

            foreach (var entity in entities)
            {
                Assert.IsTrue(entity.ExistsInWorld());
            }

            Assert.AreEqual(numberOfEntitiesToCreate, entities.Count());
            Assert.AreEqual(numberOfEntitiesToCreate, world.EntityCount());
        }  
        
        [Test]
        public void CreatesLotsOfEntities()
        {
            var entitiesToCreate = (ulong)Math.Pow(2, 13);
            var entities = world.CreateEntities(entitiesToCreate);

            foreach (var entity in entities)
            {
                Assert.IsTrue(entity.ExistsInWorld());
            }

            Assert.AreEqual(entitiesToCreate, entities.Count());
            Assert.AreEqual(entitiesToCreate, world.EntityCount());
        }  
        
        [Test]
        public void CreatesMultipleEntitiesWithComponent()
        {
            var entities = world.CreateEntitiesWithComponent<TestComponent>(75);
            var otherEntities = world.CreateEntitiesWithComponent<OtherTestComponent>(25);

            foreach (var entity in entities)
            {
                Assert.IsTrue(entity.ExistsInWorld());
                Assert.IsTrue(entity.HasComponent<TestComponent>());
                Assert.False(entity.HasComponent<OtherTestComponent>());
                Assert.AreEqual(75, entities.Count);
                Assert.AreEqual(75, world.EntityCount((ref Entity e) => e.HasComponent<TestComponent>()));

            }

            foreach (var otherEntity in otherEntities)
            {
                Assert.IsTrue(otherEntity.ExistsInWorld());
                Assert.False(otherEntity.HasComponent<TestComponent>());
                Assert.IsTrue(otherEntity.HasComponent<OtherTestComponent>());
                Assert.AreEqual(25, otherEntities.Count);
                Assert.AreEqual(25, world.EntityCount((ref Entity e) => e.HasComponent<OtherTestComponent>()));
            }
        
            Assert.AreEqual(100, world.EntityCount());
        }       
        
        
        [Test]
        public void CreatesMultipleEntitiesWithMultipleComponents()
        {
            var types = TypeList.Create()
                    .AddType<TestComponent>()
                    .AddType<OtherTestComponent>()
                    .Complete();
                
                var entities = world.CreateEntitiesWithComponents(75, types);

                var otherTypes = TypeList.Create()
                    .AddType<TestComponent>()
                    .AddType<AnotherTestComponent>()
                    .Complete();
                
                var otherEntities = world.CreateEntitiesWithComponents(25, otherTypes);
            
                
                foreach (var entity in entities)
                {
                    Assert.IsTrue(entity.ExistsInWorld());

                    var typesOnEntity = entity.GetAllAttachedComponentTypes();
                    CollectionAssert.AreEquivalent(typesOnEntity, types.Types);

                    Assert.AreEqual(75, entities.Count);
                    Assert.AreEqual(75, world.EntityCount((ref Entity e) => e.HasExactComponents(types)));
                }
                
                foreach (var otherEntity in otherEntities)
                {
                    Assert.IsTrue(otherEntity.ExistsInWorld());

                    var typesOnEntity = otherEntity.GetAllAttachedComponentTypes();
                    CollectionAssert.AreEquivalent(typesOnEntity, otherTypes.Types);

                    Assert.AreEqual(25, otherEntities.Count);
                    Assert.AreEqual(25, world.EntityCount((ref Entity e) => e.HasExactComponents(otherTypes)));
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
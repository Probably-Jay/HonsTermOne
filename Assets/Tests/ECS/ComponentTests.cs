using System;
using System.Collections.Generic;
using ECS.Internal.Exceptions;
using ECS.Internal.Extensions;
using ECS.Internal.Interfaces;
using ECS.Public;
using ECS.Public.Classes;
using ECS.Public.Interfaces;
using NUnit.Framework;
using Entity = ECS.Public.Classes.Entity;

namespace Tests.ECS
{
    public struct TestComponentValData : IComponentData
    {
        public int Data;
    }
    public class ComponentTests
    {
        private World world;
        private Entity entity;
        
        [OneTimeSetUp]
        public void OnTimeSetUp()
        {
            World.TypeRegistry.RegisterTypesFromAssemblyContaining<TestComponentValData>();
            world = new World();
        }

        [SetUp]
        public void SetUp()
        {
            entity = world.CreateEntity();
        } 
        
        [TearDown]
        public void TearDown()
        {
            entity.DestroyFromWorld();
        }
        
        [Test]
        public void CreatedComponentExists()
        {
            entity.AddComponent<TestComponentValData>();
            Assert.True(entity.HasComponent<TestComponentValData>());
        }
         
        [Test]
        public void CreateMultipleComponentA()
        {
            var types = TypeList.Create()
                .AddType<TestComponent>()
                .AddType<OtherTestComponent>()
                .Complete();

            entity.AddComponents(types);
            
            var typesOnEntity = entity.GetAllAttachedComponentTypes();
            CollectionAssert.AreEquivalent(typesOnEntity,types.Types);
        }  
        
        [Test]
        public void CreateMultipleComponentB()
        {
            var types = new MyType();

            entity.AddComponents(types);
            
            var typesOnEntity = entity.GetAllAttachedComponentTypes();
            CollectionAssert.AreEquivalent(typesOnEntity,types.Types);
        }

       
        
        [Test]
        public void CreateMultipleComponentC()
        {
            entity.AddComponents<MyType>();
            
            var typesOnEntity = entity.GetAllAttachedComponentTypes();
            CollectionAssert.AreEquivalent(typesOnEntity,new MyType().Types);
        } 
        
        class EmptyType : TypeList
        {
        }

        [Test]
        public void DoesNotCreateMultipleComponentWhenEmptyTypeIsUsed()
        {
            entity.AddComponents<EmptyType>();
            
            var typesOnEntity = entity.GetAllAttachedComponentTypes();
            CollectionAssert.AreEquivalent(typesOnEntity,new Type[]{});
        }

        class BadType : TypeList
        {
            public BadType() : base(
                () => typeof(int)
                ) { }
        }
        
        [Test]
        public void DoesNotCreateMultipleComponentWhenBadTypeIsUsed()
        {
            try
            {
                entity.AddComponents<BadType>();
                Assert.Fail();
            }
            catch
            {
                //
            }
        }

        [Test]
        public void CreateMultipleSameComponentFailsA()
        {
            Assert.Throws<DuplicateTypesInTypeListException>(
                () => TypeList.Create()
                .AddType<TestComponent>()
                .AddType<TestComponent>()
                .Complete());
        }

        class BadTypeDuplicate : TypeList
        {
            public BadTypeDuplicate() : base(
                Type<TestComponent>,
                Type<TestComponent>
            )
            { }
        }

        [Test]
        public void CreateMultipleSameComponentFailsB()
        {
            Assert.Throws<DuplicateTypesInTypeListException>(
                () =>_ = new BadTypeDuplicate());
        }
        
        
        [Test]
        public void ComponentValueIsUpdatable()
        {
            entity.AddComponent<TestComponentValData>();
            
            var initialVal = entity.ReadComponent<TestComponentValData>().ComponentData.Data;
            
            Assert.AreEqual(initialVal, 0);

            entity.ModifyComponentData((ref TestComponentValData data) =>
            {
                data.Data++;
            });
            
            var newValue = entity.ReadComponent<TestComponentValData>().ComponentData.Data;
            
            Assert.AreEqual(newValue, 1);
        }

        [Test]
        public void ComponentsRemovedDestroySelf()
        {
            Assert.False(entity.HasComponent<TestComponentValData>());
            
            entity.AddComponent<TestComponentValData>();
            Assert.True(entity.HasComponent<TestComponentValData>());
            entity.RemoveComponent<TestComponentValData>();
            
            Assert.False(entity.HasComponent<TestComponentValData>());
        }  
        
        [Test]
        public void ComponentsRemovedDestroyedFromWorld()
        {
            entity.AddComponent<TestComponentValData>();

            Assert.True(entity.QueryComponent(
                (ref ComponentEcs<TestComponentValData> component) =>
                    component.ExistsAttachedToEntity())
            );
            
            entity.RemoveComponent<TestComponentValData>();
            
            Assert.Throws<EntityDoesNotContainComponentException>(
                    () => _ = entity.QueryComponent(
                        (ref ComponentEcs<TestComponentValData> component) =>
                            component.ExistsAttachedToEntity()
                            )
                    );
        } 
        

        [Test]
        public void ComponentsRemovedResets() {
            entity.AddComponent<TestComponentValData>();

            entity.ModifyComponentData(
                (ref TestComponentValData component) =>
                {
                    component.Data = 1;
                });
            
            Assert.AreEqual(entity.ReadComponent<TestComponentValData>().ComponentData.Data, 1);
            
            entity.RemoveComponent<TestComponentValData>();
            entity.AddComponent<TestComponentValData>();
            
            Assert.AreEqual(entity.ReadComponent<TestComponentValData>().ComponentData.Data, 0);
        }

        [Test]
        public void ComponentOverwriteOverwrites()
        {
            entity.AddComponent<TestComponentValData>();

            var component = entity.ReadComponent<TestComponentValData>();

            component.ComponentData.Data = 1;
            
            Assert.AreEqual(entity.ReadComponent<TestComponentValData>().ComponentData.Data, 0);

            entity.WriteComponent(component);
            
            Assert.AreEqual(entity.ReadComponent<TestComponentValData>().ComponentData.Data, 1);
        }
    }
}
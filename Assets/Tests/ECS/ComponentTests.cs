using ECS.Scripts.Real.Internal.Exceptions;
using ECS.Scripts.Real.Internal.Extentions;
using ECS.Scripts.Real.Internal.Interfaces;
using ECS.Scripts.Real.Public;
using NUnit.Framework;

namespace Tests.ECS
{
    public struct TestComponentValData : IComponentData
    {
        public int Data;
    }
    class ComponentTests
    {
        private World world;
        private Entity entity;
        
        [OneTimeSetUp]
        public void OnTimeSetUp()
        {
            world = new World();
            world.RegisterEntityTypes<TestComponentValData>();
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
        public void CreateComponent()
        {
            ref var component = ref entity.AddComponent<TestComponentValData>();
            Assert.False(component.IsNullComponent());
        } 
        
        [Test]
        public void CreatedComponentExists()
        {
            ref var component = ref entity.AddComponent<TestComponentValData>();
            Assert.True(component.ExistsAttachedToEntity());
        }

        [Test]
        public void GetComponentGetsActualComponent()
        {
            ref var createdComponent = ref entity.AddComponent<TestComponentValData>();
            ref var gotComponent = ref entity.GetComponent<TestComponentValData>();
            
            Assert.True(gotComponent.ExistsAttachedToEntity());
            Assert.True(createdComponent.Equals(gotComponent));
            Assert.AreEqual(createdComponent, gotComponent);
        }
        
        [Test]
        public void ComponentValueIsUpdatable()
        {
            {
                ref var component = ref entity.AddComponent<TestComponentValData>();

                Assert.AreEqual(component.ComponentData.Data, 0);

                component.ComponentData.Data++;
                Assert.AreEqual(component.ComponentData.Data, 1);
            }
            {
                ref var component = ref entity.GetComponent<TestComponentValData>();

                Assert.AreEqual(component.ComponentData.Data, 1);

                component.ComponentData.Data++;
                Assert.AreEqual(component.ComponentData.Data, 2);
            }
        }

        [Test]
        public void ComponentsRemovedDestroySelf()
        {
            ref var component = ref entity.AddComponent<TestComponentValData>();
            entity.RemoveComponent<TestComponentValData>();
            
            Assert.True(component.IsNullComponent());
        }  
        
        [Test]
        public void ComponentsRemovedDestroyedFromWorld()
        {
            ref var component = ref entity.AddComponent<TestComponentValData>();
            entity.RemoveComponent<TestComponentValData>();
            
            Assert.False(component.ExistsAttachedToEntity());
        } 
        
        [Test]
        public void ComponentsRemovedCannotBeAccessed()
        {
            Assert.Throws<ComponentNullException>(() => entity.GetComponent<TestComponentValData>());

            entity.AddComponent<TestComponentValData>();
            entity.RemoveComponent<TestComponentValData>();
            
           Assert.Throws<ComponentNullException>(() => entity.GetComponent<TestComponentValData>());
        } 
        
        // [Test]
        // public void ComponentsRemovedCannotModify()
        // {
        //     ref var component = ref entity.AddComponent<TestComponentValData>();
        //     entity.RemoveComponent<TestComponentValData>();
        //     
        //     Assert.False(component.ExistsAttachedToEntity());
        // }
    }
}
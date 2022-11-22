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
        public void CreateMultipleComponent()
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
                (ref Component<TestComponentValData> component) =>
                    component.ExistsAttachedToEntity())
            );
            
            entity.RemoveComponent<TestComponentValData>();
            
            Assert.Throws<EntityDoesNotContainComponentException>(
                    () => _ = entity.QueryComponent(
                        (ref Component<TestComponentValData> component) =>
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
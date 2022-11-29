using System;
using System.Collections.Generic;
using System.Linq;
using ECS.Internal.Extensions;
using ECS.Internal.Interfaces;
using ECS.Public;
using ECS.Public.Attributes;
using ECS.Public.Classes;
using ECS.Public.Extensions;
using ECS.Public.Interfaces;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;
using UnityEditorInternal;

namespace Tests.ECS
{
  

    public interface IDataModule
    {
        void Floop();
    }


    [SystemOperatesOn()]
    public class TrivialTestSystem : ISystemLogic
    {
        public IDataModule DataModule { get; private set; } = null;

        public void SetUp(IDataModule data)
        {
            DataModule = data;
        }
    
        public void Update(float deltaTime, ISystemEntityView entityView)
        {
            DataModule.Floop();
        }
    }
    
    [SystemOperatesOn(Exactly = new []{typeof(TestComponent), typeof(OtherTestComponent)} )]
    public class ActualTestSystem : ISystemLogic
    {
        public void Update(float deltaTime, ISystemEntityView entityView)
        {
            ref var test = ref entityView.GetComponent<TestComponent>();
            ref var other = ref entityView.GetComponent<OtherTestComponent>();

            other.Data++;
        }
    }
    
    public struct AComponent : IComponentData
    {
        public int Data;
    }
    
    [SystemOperatesOn(Contains = new []{typeof(AComponent)} )]
    public class ContainsA : ISystemLogic
    {
        public void Update(float deltaTime, ISystemEntityView entityView)
        {
            ref var aComponent = ref entityView.GetComponent<AComponent>();
            aComponent.Data++;
        }
    } 
    
    public struct BComponent : IComponentData
    {
        public int Data;
    }public struct CComponent : IComponentData
    {
        public int Data;
    }
    
    [SystemOperatesOn(Contains = new []{typeof(BComponent), typeof(CComponent)} )]
    public class ContainsBAndC : ISystemLogic
    {
        public void Update(float deltaTime, ISystemEntityView entityView)
        {
            ref var bComponent = ref entityView.GetComponent<BComponent>();
            ref var cComponent = ref entityView.GetComponent<CComponent>();

            bComponent.Data++;
            cComponent.Data++;
        }
    }  
    
    public struct DComponent : IComponentData
    {
        public int Data;
    }public struct EComponent : IComponentData
    {
        public int Data;
    }
    
    [SystemOperatesOn(Contains = new []{typeof(DComponent)}, Without = new []{typeof(EComponent)})]
    public class ContainsDWithoutE : ISystemLogic
    {
        public void Update(float deltaTime, ISystemEntityView entityView)
        {
            ref var dComponent = ref entityView.GetComponent<DComponent>();

            dComponent.Data++;
        }
    }
    
    public class MyType : TypeList
    {
        public MyType() : base(
            Type<TestComponent>, 
            Type<OtherTestComponent>
        ) { }
    }

    public class SystemTests
    {
        private IDataModule dataModule;
        private World world;
        private TrivialTestSystem system;
        
        [OneTimeSetUp]
        public void OnTimeSetUp()
        {
            world = new World();
            World.TypeRegistry.RegisterTypesFromAssemblyContaining<IAssemblyMarker>();
        }  
        
        [SetUp]
        public void SetUp()
        {
            dataModule = Substitute.For<IDataModule>();
            world.ModifySystem((TrivialTestSystem s) => s.SetUp(dataModule));
        }
        
        
        [Test]
        public void SetUpWorks()
        {
            Assert.NotNull(world.QuerySystem((TrivialTestSystem s) => s.DataModule));
        }
        
        [Test]
        public void SystemCanTickTrivially()
        {
            world.CreateEntity();
            world.Tick(1/50f);
            dataModule.Received(1).Floop();
        }
        
        
        [Test]
        public void ActualSystemCanTickEntityWhenComponentsMatch()
        {
            var entity = world.CreateEntitiesWithComponents(10, new MyType()).ToList();
            world.Tick(1/50f);

            Assert.AreEqual(1, entity[0].ReadComponent<OtherTestComponent>().Data);
            
            world.Tick(1/50f);
            
            Assert.AreEqual(2, entity[0].ReadComponent<OtherTestComponent>().Data);
        } 
        
        [Test]
        public void ActualSystemDoesNotTickEntityWhenComponentsDontMatch()
        {
            var entity = world.CreateEntitiesWithComponent<OtherTestComponent>(10).ToList();
            world.Tick(1/50f);

            Assert.AreEqual(0, entity[0].ReadComponent<OtherTestComponent>().Data);
            
            world.Tick(1/50f);
            
            Assert.AreEqual(0, entity[0].ReadComponent<OtherTestComponent>().Data);
        }

        [Test]
        public void ContainsASystemTicksWhenEntityComponentsMatch()
        {
            {
                var type = TypeList.Create().AddType<AComponent>().Complete();
                var entity = world.CreateEntitiesWithComponents(10, type).ToList();
                world.Tick(1 / 50f);

                Assert.AreEqual(1, entity[0].ReadComponent<AComponent>().Data);
            }
            world.DestroyAllEntities();
            {
                var type = TypeList.Create().AddType<AComponent>().AddType<BComponent>().Complete();
                var entity = world.CreateEntitiesWithComponents(10, type).ToList();
                world.Tick(1 / 50f);

                Assert.AreEqual(1, entity[0].ReadComponent<AComponent>().Data);
            }
        }

        [Test]
        public void ContainsBandCSystemTicksWhenEntityComponentsMatch()
        {
            {
                var type = TypeList.Create().AddType<BComponent>().AddType<CComponent>().Complete();
                var entity = world.CreateEntitiesWithComponents(10, type).ToList();
                world.Tick(1 / 50f);

                Assert.AreEqual(1, entity[0].ReadComponent<BComponent>().Data);
                Assert.AreEqual(1, entity[0].ReadComponent<CComponent>().Data);
            }
            world.DestroyAllEntities();
            {
                var type = TypeList.Create().AddType<BComponent>().AddType<CComponent>().AddType<DComponent>().Complete();
                var entity = world.CreateEntitiesWithComponents(10, type).ToList();
                world.Tick(1 / 50f);

                Assert.AreEqual(1, entity[0].ReadComponent<BComponent>().Data);
                Assert.AreEqual(1, entity[0].ReadComponent<CComponent>().Data);
            }
        }

        [Test]
        public void ContainsDWithoutESystemTicksWhenEntityComponentsMatch()
        {
            {
                var type = TypeList.Create().AddType<DComponent>().Complete();
                var entity = world.CreateEntitiesWithComponents(10, type).ToList();
                world.Tick(1 / 50f);

                Assert.AreEqual(1, entity[0].ReadComponent<DComponent>().Data);
            }
            world.DestroyAllEntities();
            {
                var type = TypeList.Create().AddType<AComponent>().AddType<DComponent>().Complete();
                var entity = world.CreateEntitiesWithComponents(10, type).ToList();
                world.Tick(1 / 50f);

                Assert.AreEqual(1, entity[0].ReadComponent<DComponent>().Data);
            }
        } 
        
        [Test]
        public void ContainsDWithoutESystemDoesNotTickWhenEntityComponentsDontMatch()
        {
            {
                var type = TypeList.Create().AddType<DComponent>().AddType<EComponent>().Complete();
                var entity = world.CreateEntitiesWithComponents(10, type).ToList();
                world.Tick(1 / 50f);

                Assert.AreEqual(0, entity[0].ReadComponent<DComponent>().Data);
            }
            world.DestroyAllEntities();
            {
                var type = TypeList.Create().AddType<EComponent>().Complete();
                var entity = world.CreateEntitiesWithComponents(10, type).ToList();
                world.Tick(1 / 50f);

                try
                {
                    _ = entity[0].ReadComponent<DComponent>().Data;
                    Assert.Fail();
                }
                catch (Exception)
                {
                    //
                }
            }
        }

        [TearDown]
        public void Teardown()
        {
            world.DestroyAllEntities();
        }
    }
}
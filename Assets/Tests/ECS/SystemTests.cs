using System;
using System.Collections.Generic;
using System.Linq;
using ECS.Internal.Extensions;
using ECS.Internal.Interfaces;
using ECS.Public;
using ECS.Public.Attributes;
using ECS.Public.Classes;
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
    
    [SystemOperatesOn(typeof(TestComponent), typeof(OtherTestComponent) )]
    public class ActualTestSystem : ISystemLogic
    {
        public void Update(float deltaTime, ISystemEntityView entityView)
        {
            ref var test = ref entityView.GetComponent<TestComponent>();
            ref var other = ref entityView.GetComponent<OtherTestComponent>();

            other.Data++;
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
        public void SystemCanTickEntityWhenComponentsMatch()
        {
            var entity = world.CreateEntities(10, new MyType()).ToList();
            world.Tick(1/50f);

            Assert.AreEqual(1, entity[0].ReadComponent<OtherTestComponent>().ComponentData.Data);
            
            world.Tick(1/50f);
            
            Assert.AreEqual(2, entity[0].ReadComponent<OtherTestComponent>().ComponentData.Data);
        } 
        
        [Test]
        public void SystemDoesNotTickEntityWhenComponentsDontMatch()
        {
            var entity = world.CreateEntities<OtherTestComponent>(10).ToList();
            world.Tick(1/50f);

            Assert.AreEqual(0, entity[0].ReadComponent<OtherTestComponent>().ComponentData.Data);
            
            world.Tick(1/50f);
            
            Assert.AreEqual(0, entity[0].ReadComponent<OtherTestComponent>().ComponentData.Data);
        }

        [TearDown]
        public void Teardown()
        {
            world.DestroyAllEntities();
        }
    }
}
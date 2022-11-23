using ECS.Scripts.Real.Internal.Interfaces;
using ECS.Scripts.Real.Public;
using ECS.Scripts.Real.Public.Attributes;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;
using UnityEditorInternal;

namespace Tests.ECS
{

    public interface IDataModule
    {
        void Init();
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
    
        public void Update(float deltaTime, IComponentUpdateContainer entityComponentContainer)
        {
            DataModule.Floop();
        }
    }
    
    [SystemOperatesOn(typeof(TestComponent), typeof(OtherTestComponent) )]
    public class ActualTestSystem : ISystemLogic
    {
    
        public void Update(float deltaTime, IComponentUpdateContainer entityComponentContainer)
        {
            ref var a = ref entityComponentContainer.Get<TestComponent>();
        }
    
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

        [TearDown]
        public void Teardown()
        {
            world.DestroyAllEntities();
        }
    }
}
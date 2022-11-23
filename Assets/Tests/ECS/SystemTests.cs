using ECS.Scripts.Real.Public;
using NSubstitute;
using NUnit.Framework;
using UnityEditorInternal;

namespace Tests.ECS
{

    public interface IDataModule
    {
        void Init();
        void Floop();
    }
    
    
    public class TestSystem : ISystemLogic
    {
        public IDataModule DataModule { get; private set; } = null;

        public void SetUp(IDataModule data)
        {
            DataModule = data;
        }
        public void Update(float deltaTime)
        {
            DataModule.Floop();
        }
    }

    public class SystemTests
    {
        private IDataModule dataModule;
        private World world;
        private TestSystem system;
        
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
            world.ModifySystem((TestSystem s) => s.SetUp(dataModule));
        }
        
        
        [Test]
        public void SetUpWorks()
        {
            Assert.NotNull(world.QuerySystem((TestSystem s) => s.DataModule));
        }
        
    }
}
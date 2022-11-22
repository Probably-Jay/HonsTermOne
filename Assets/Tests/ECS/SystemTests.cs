using ECS.Scripts.Real.Public;
using NUnit.Framework;
using UnityEditorInternal;

namespace Tests.ECS
{

    public class TestSystem : ISystemLogic
    {
        public void Update(float deltaTime)
        {
            throw new System.NotImplementedException();
        }
    }

    public class SystemTests
    {
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
      
        }

        [Test]
        public void CreateSystem()
        {
            
        }
        
    }
}
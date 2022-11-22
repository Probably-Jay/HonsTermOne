using ECS.Scripts.Real.Public;
using NUnit.Framework;
using UnityEditorInternal;

namespace Tests.ECS
{

    public class TestSystem : global::ECS.Scripts.Real.Public.System
    {
        
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
            system = new TestSystem();
        }

        [Test]
        public void CreateSystem()
        {
            
        }
        
    }
}
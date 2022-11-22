using System;
using ECS.Scripts.Real.Internal.Interfaces;
using ECS.Scripts.Real.Public;
using NUnit.Framework;

namespace Tests.ECS
{
    public struct TestComponent : IComponentData 
    {
    
    }

    static class TypeRegistry
    {
        public static readonly Type[] AllIComponentTypes = {
            typeof(TestComponent),
            typeof(TestComponentValData)
        };
    }
    public class WorldTests
    {
        private World world;

        [OneTimeSetUp]
        public void Setup()
        {
            world = new World();
        }
    
        [Test]
        public void WorldCreation()
        {
        } 
    
        [Test]
        public void RegisterComponent()
        {
            world.RegisterEntityTypes<TestComponent>();

            var registeredComponents = world.GetAllRegisteredComponentTypes();
        
            CollectionAssert.AreEquivalent(TypeRegistry.AllIComponentTypes, registeredComponents);
        }
    
    }
}


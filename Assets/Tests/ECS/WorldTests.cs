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
        
        public static readonly Type[] AllSystemTypes = {
            typeof(TestSystem),
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
            World.TypeRegistry.RegisterTypesFromAssemblyContaining<IAssemblyMarker>();
            
            var registeredComponents = World.TypeRegistry.ComponentTypes;
        
            CollectionAssert.AreEquivalent(TypeRegistry.AllIComponentTypes, registeredComponents);
        }
     
        [Test]
        public void RegisterSystems()
        {
            World.TypeRegistry.RegisterTypesFromAssemblyContaining<IAssemblyMarker>();

            var registeredComponents = World.TypeRegistry.SystemTypes;
        
            CollectionAssert.AreEquivalent(TypeRegistry.AllSystemTypes, registeredComponents);
        }
        
    }
}


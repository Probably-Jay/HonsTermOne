using System;
using ECS.Internal.Interfaces;
using ECS.Public;
using ECS.Public.Classes;
using ECS.Public.Interfaces;
using NUnit.Framework;

namespace Tests.ECS
{
    public struct TestComponent : IComponentData 
    {
    
    }  
    
    public struct OtherTestComponent : IComponentData
    {
        public int Data;
    }
    
    public struct AnotherTestComponent : IComponentData 
    {
    
    }

    static class TypeRegistry
    {
        public static readonly Type[] AllIComponentTypes = {
            typeof(TestComponent),
            typeof(OtherTestComponent),
            typeof(TestComponentValData),
            typeof(AnotherTestComponent),
            typeof(AComponent),
            typeof(BComponent),
            typeof(CComponent),
            typeof(DComponent),
            typeof(EComponent),
        }; 
        
        public static readonly Type[] AllSystemTypes = {
            typeof(TrivialTestSystem),
            typeof(ActualTestSystem),
            typeof(ContainsA),
            typeof(ContainsBAndC),
            typeof(ContainsDWithoutE),
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


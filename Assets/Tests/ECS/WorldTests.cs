using System.Collections;
using System.Collections.Generic;
using ECS.Scripts.Real.Internal.Interfaces;
using ECS.Scripts.Real.Public;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

public struct TestComponent : IComponentData 
{
    
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
        
        CollectionAssert.AreEqual(new[]{typeof(TestComponent)}, registeredComponents);
    }
    
    

   
}

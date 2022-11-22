using ECS.Scripts.Real.Internal.Extentions;
using ECS.Scripts.Real.Internal.Interfaces;
using ECS.Scripts.Real.Public;
using ECS.Scripts.Real.Public.Attributes;
using UnityEngine;

namespace ECS.Scripts.New_Folder
{
    public class TestClass : MonoBehaviour
    {
       // private ComponentAnymap container;

       private readonly World world = new();

        private void Awake()
        {
            world.RegisterEntityTypes<IAssemblyMarker>();
        }

        private void Start()
        {
            Test();
        }

        private void Test()
        {
            Entity entity = world.CreateEntity();
            
            entity.AddComponent<Foo>();
            

            ref var data = ref entity.GetComponent<Foo>();
            Debug.Log(data);

            data.ComponentData = new Foo{data = 1};
            
            ref var data2 = ref entity.GetComponent<Foo>();
            Debug.Log(data2);
            
            entity.DestroyFromWorld();

            
            data2.ComponentData.data++;
            
            ref var data3 = ref entity.GetComponent<Foo>();
            Debug.Log(data3);
            Debug.Log(data);
            

            Debug.Log(data.Exists);

        }
    }

    [ReserveInComponentArray(2000)]
    public struct Foo : IComponentData
    {
        public int data { get; set; }
        
        public override string ToString()
        {
            return data.ToString();
        }
    }
}
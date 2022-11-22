using ECS.Scripts.Real.Interfaces;
using ECS.Scripts.Real.Internal.Extentions;
using ECS.Scripts.Real.Types;
using ECS.Scripts.Real.Types.Attributes;
using UnityEngine;

namespace ECS.Scripts.New_Folder
{
    public class TestClass : MonoBehaviour
    {
       // private ComponentAnymap container;


        private void Awake()
        {
       //     container = new ComponentAnymap();
        }

        private void Start()
        {
            Test();
        }

        private void Test()
        {
            var entity = World.CreateEntity();
            entity.AddComponent<Foo>();
            

            ref var data = ref entity.GetComponent<Foo>();
            Debug.Log(data);

            data.Data = new Foo{data = 1};
            
            ref var data2 = ref entity.GetComponent<Foo>();
            Debug.Log(data2);
            
            data2.Data.data++;
            
            ref var data3 = ref entity.GetComponent<Foo>();
            Debug.Log(data3);
            Debug.Log(data);
            
            
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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ECS.Scripts.New_Folder;
using UnityEngine;

namespace ECS.Scripts.Real
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

            data.Component.data++;
            
            ref EntityComponentWrapped<Foo> data2 = ref entity.GetComponent<Foo>();
            Debug.Log(data2);
            
            data2.Component.data++;
            
            ref var data3 = ref entity.GetComponent<Foo>();
            Debug.Log(data3);
            Debug.Log(data);
            
            
        }
    }

    [ReserveInComponentArray(2000)]
    public struct Foo : IComponentECS
    {
        public int data { get; set; }
        
        public override string ToString()
        {
            return data.ToString();
        }
    }
}
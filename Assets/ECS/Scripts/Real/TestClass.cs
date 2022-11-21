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

            data.data = 1;
            
            ref var data2 = ref entity.GetComponent<Foo>();
            Debug.Log(data);
        }
    }

    [ReserveInComponentArray(2000)]
    public struct Foo : IComponentECS
    {
        public int data { get; set; }
        
        public Entity EntityID { get; private set; }
        public void SetEntity(Entity entity)
        {
            EntityID = entity;
        }

        public override string ToString()
        {
            return data.ToString();
        }
    }
}
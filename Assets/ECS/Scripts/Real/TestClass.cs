using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ECS.Scripts.New_Folder;
using UnityEngine;

namespace ECS.Scripts.Real
{
    public class TestClass : MonoBehaviour
    {
        private ComponentAnymap container;


        private void Awake()
        {
            container = new ComponentAnymap();
        }

        private void Start()
        {
            Test();
        }

        private void Test()
        {
            var entity = new Entity(new GenerationalID(1, 0));
            var f = new Foo() { data = 1, EntityID = entity  };
            container.Add(f);

            ref var data = ref container.Get<Foo>(entity);
            Debug.Log(data);

            data.data = 2;
            
            ref var data2 = ref container.Get<Foo>(entity);
            Debug.Log(data);
        }
    }

    public struct Foo : IComponentECS
    {
        public int data { get; set; }
        
        public Entity EntityID { get; set; }

        public override string ToString()
        {
            return data.ToString();
        }
    }
}
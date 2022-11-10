using System;
using ECS.Scripts.New_Folder;
using UnityEngine;

namespace ECS.Scripts.Real
{
    public class TestClass : MonoBehaviour
    {
        private ComponentArrayContainer container = new();

        private void Awake()
        {
            container.ScanForTypes();
        }

        private void Start()
        {
            var f = new Foo() {data = true};
            container.Add(f);

            var data = container.GetList<Foo>()![0].data;
            Debug.Log(data);
        }
    }

    public struct Foo : IComponentECS
    {
        public bool data { get; set; }
        public bool Equals(IComponentECS other)
        {
            throw new NotImplementedException();
        }
    }
}
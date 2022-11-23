using ECS.Public.Attributes;
using ECS.Public.Classes;
using ECS.Public.Interfaces;
using UnityEngine;

namespace ECS.Scripts.New_Folder
{
    public class TestClass : MonoBehaviour
    {
      
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
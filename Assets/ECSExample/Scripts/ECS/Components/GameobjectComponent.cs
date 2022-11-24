using ECS.Public.Interfaces;
using UnityEngine;

namespace ECSExample.Scripts.Static
{
    public struct GameobjectComponent : IComponentData
    {
        public GameObject GameObject;
    }
    
    // public struct GameobjectComponent : IComponentData
    // {
    //     public GameObject GameObject;
    // }
}
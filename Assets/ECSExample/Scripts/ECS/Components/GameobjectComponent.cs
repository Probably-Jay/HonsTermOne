using ECS.Public.Interfaces;
using UnityEngine;

namespace ECSExample.Scripts.ECS.Components
{
    public struct GameobjectComponent : IComponentData
    {
        public GameObject GameObject;
    }
    
    public struct PositionComponent : IComponentData
    {
        public Vector3 Position;
    }
    
    public struct VelocityComponent : IComponentData
    {
        public Vector3 Velocity;
    }
}
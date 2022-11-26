using ECS.Public.Attributes;
using ECS.Public.Classes;
using ECS.Public.Interfaces;
using ECSExample.Scripts.ECS.Components;
using JetBrains.Annotations;
using UnityEngine;

namespace ECSExample.Scripts.ECS.Systems
{
    [UsedImplicitly]
    [SystemOperatesOn( Contains = new [] { typeof(PositionComponent), typeof(VelocityComponent) })]
    public class MoveGameObjectSystem : ISystemLogic
    {
        public GameObject RootGameObject { get; set; }
        
        public void Update(float deltaTime, ISystemEntityView entityView)
        {
            ref var position = ref entityView.GetComponent<PositionComponent>();
            ref var velocity = ref entityView.GetComponent<VelocityComponent>();

            var directionToRoot = Vector3.ClampMagnitude((RootGameObject.transform.position - position.Position), 1);

            velocity.Velocity += directionToRoot * deltaTime;

            position.Position += velocity.Velocity * deltaTime;
        }
    }
}

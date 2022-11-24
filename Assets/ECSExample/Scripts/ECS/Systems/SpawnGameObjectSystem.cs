using ECS.Public.Attributes;
using ECS.Public.Classes;
using ECS.Public.Interfaces;
using ECSExample.Scripts.Static;
using JetBrains.Annotations;
using UnityEngine;

namespace ECSExample.Scripts.ECS.Systems
{
    [SystemOperatesOn( Contains = new [] { typeof(GameobjectComponent), typeof(PositionComponent) })]
    public class SpawnGameObjectSystem : ISystemLogic
    {
        public GameObject RootGameObject { get; set; }
        
        public void Update(float deltaTime, ISystemEntityView entityView)
        {
            ref var gameobjectComponent = ref entityView.GetComponent<GameobjectComponent>();
            if(gameobjectComponent.GameObject != null)
                return;
            
            ref var positionComponent = ref entityView.GetComponent<PositionComponent>();

            var r = new System.Random((int)entityView.Entity.EntityIDIndex);

            var position = new Vector3(r.Next(-10, 10), r.Next(-10, 10), r.Next(-10, 10));
            positionComponent.Position = position;

            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.transform.SetParent(RootGameObject.transform);
            gameObject.transform.position = position;
            gameobjectComponent.GameObject = gameObject;
        }
    }
    
    
    
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
    
    [SystemOperatesOn( Contains = new [] { typeof(PositionComponent), typeof(GameobjectComponent) })]
    public class ApplyGameObjectPosition : ISystemLogic
    {
        public void Update(float deltaTime, ISystemEntityView entityView)
        {
            ref var gameobjectComponent = ref entityView.GetComponent<GameobjectComponent>();
            if(gameobjectComponent.GameObject == null)
                return;

            ref var position = ref entityView.GetComponent<PositionComponent>();

            gameobjectComponent.GameObject.transform.position = position.Position;
        }
    }
}
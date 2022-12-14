using ECS.Public.Attributes;
using ECS.Public.Classes;
using ECS.Public.Interfaces;
using ECSExample.Scripts.ECS.Components;
using ECSExample.Scripts.Static;
using JetBrains.Annotations;
using UnityEngine;

namespace ECSExample.Scripts.ECS.Systems
{
    [UsedImplicitly]
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

            var r = new System.Random((int)entityView.Entity.GetHashCode());
            
            var position = new Vector3(r.Next(-10, 10), r.Next(-10, 10), r.Next(-10, 10));
            positionComponent.Position = position;

            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.transform.SetParent(RootGameObject.transform);
            gameObject.transform.position = position;
            gameobjectComponent.GameObject = gameObject;
        }
    }


}
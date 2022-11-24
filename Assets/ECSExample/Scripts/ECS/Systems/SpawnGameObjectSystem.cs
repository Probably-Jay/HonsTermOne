using ECS.Public.Attributes;
using ECS.Public.Classes;
using ECS.Public.Interfaces;
using UnityEngine;

namespace ECSExample.Scripts.Static
{
    [SystemOperatesOn( Contains = new [] { typeof(GameobjectComponent) })]
    public class SpawnGameObjectSystem : ISystemLogic
    {
        public GameObject Root { get; set; }
        
        public void Update(float deltaTime, ISystemEntityView entityView)
        {
            ref var component = ref entityView.GetComponent<GameobjectComponent>();
            if(component.GameObject != null)
                return;

            var r = new System.Random((int)entityView.Entity.EntityIDIndex);

            var position = new Vector3(r.Next(-10, 10), r.Next(-10, 10), r.Next(-10, 10));

            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.transform.SetParent(Root.transform);
            gameObject.transform.position = position;
            component.GameObject = gameObject;
        }
    }
}
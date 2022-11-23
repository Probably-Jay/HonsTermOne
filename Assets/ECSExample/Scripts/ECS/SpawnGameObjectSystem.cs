using ECS.Public.Attributes;
using ECS.Public.Classes;
using ECS.Public.Interfaces;
using UnityEngine;

namespace ECSExample.Scripts.Static
{
    [SystemOperatesOn(typeof(SpawnGameobjectComponent))]
    public class SpawnGameObjectSystem : ISystemLogic
    {
        public GameObject Root { get; set; }
        
        public void Update(float deltaTime, ISystemEntityView entityView)
        {
            ref var component = ref entityView.GetComponent<SpawnGameobjectComponent>();
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
    
    public struct SpawnGameobjectComponent : IComponentData
    {
        public int Spawned;
        public GameObject GameObject;
    }
}
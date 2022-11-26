using ECS.Public.Attributes;
using ECS.Public.Classes;
using ECS.Public.Interfaces;
using ECSExample.Scripts.ECS.Components;
using JetBrains.Annotations;

namespace ECSExample.Scripts.ECS.Systems
{
    [UsedImplicitly]
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

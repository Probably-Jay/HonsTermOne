using ECSExample.Scripts.ECS.Systems;
using ECSExample.Scripts.Static;
using UnityEngine;

namespace ECSExample.Scripts.Monobehaviours
{
    public class WorldUpdater : MonoBehaviour
    {
        [SerializeField] private WorldProvider worldProvider;

        private void Start()
        {
            var rootObject = new GameObject();
            worldProvider.World.ModifySystem<SpawnGameObjectSystem>(system => system.RootGameObject = rootObject);
            worldProvider.World.ModifySystem<MoveGameObjectSystem>(system => system.RootGameObject = rootObject);
        }

        private void Update()
        {
            worldProvider.World.Tick(Time.deltaTime);
        }
    }
}

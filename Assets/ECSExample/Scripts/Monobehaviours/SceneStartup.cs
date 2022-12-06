using ECS.Public.Extensions;
using ECSExample.Scripts.ECS.Systems;
using ECSExample.Scripts.Static;
using UnityEngine;

namespace ECSExample.Scripts.Monobehaviours
{
    public class SceneStartup : MonoBehaviour
    {
        [SerializeField] private WorldProvider worldProvider;
        [SerializeField] private EntityCreator entityCreator;

        private void Awake()
        {
            worldProvider.Initialise();
            
            var rootObject = new GameObject();
            worldProvider.World.ModifySystem<SpawnGameObjectSystem>(system => system.RootGameObject = rootObject);
            worldProvider.World.ModifySystem<MoveGameObjectSystem>(system => system.RootGameObject = rootObject);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                entityCreator.CreateEntities();
            }
        }
    }
}

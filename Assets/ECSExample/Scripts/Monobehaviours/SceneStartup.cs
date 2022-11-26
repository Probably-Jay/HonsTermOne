using ECS.Public.Extensions;
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
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                entityCreator.CreateEntities();
                var world = worldProvider.World;

                var entities = world.EntityCount();
                Debug.Log($"Entities {entities} exist");
            }
        }
    }
}

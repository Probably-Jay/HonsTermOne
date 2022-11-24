using ECS.Public.Interfaces;
using ECSExample.Scripts.ECS.TypeDefinitions;
using UnityEngine;

namespace ECSExample.Scripts.Static
{
    [CreateAssetMenu(fileName = "EntityCreator", menuName = "StaticObjects/EntityCreator")]
    public class EntityCreator : ScriptableObject
    {
        [SerializeField] private WorldProvider worldProvider;
        [SerializeField] private ulong entitiesToCreate;

        public void CreateEntities()
        {
            worldProvider.World.CreateEntitiesWithComponents<CubeType>(entitiesToCreate);
        }
    }


   
}
using UnityEngine;

namespace ECSExample.Scripts.Static
{
    [CreateAssetMenu(fileName = "EntityCreator", menuName = "StaticObjects/EntityCreator")]
    public class EntityCreator : ScriptableObject
    {
        [SerializeField] private WorldProvider worldProvider;
        [SerializeField] private int entitiesToCreate;

        public void CreateEntities()
        {
            worldProvider.World.CreateEntities(entitiesToCreate);
        }
    }
}
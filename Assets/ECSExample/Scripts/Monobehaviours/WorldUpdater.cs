using ECSExample.Scripts.ECS.Systems;
using ECSExample.Scripts.Static;
using UnityEngine;

namespace ECSExample.Scripts.Monobehaviours
{
    public class WorldUpdater : MonoBehaviour
    {
        [SerializeField] private WorldProvider worldProvider;
        
        private void Update()
        {
            worldProvider.World.Tick(Time.deltaTime);
        }
    }
}

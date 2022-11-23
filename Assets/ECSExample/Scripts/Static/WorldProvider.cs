using ECS.Public.Classes;
using ECSExample.Scripts.Example;
using UnityEngine;

namespace ECSExample.Scripts.Static
{
    [CreateAssetMenu(fileName = "WorldProvider", menuName = "StaticObjects/WorldProvider")]
    public class WorldProvider : ScriptableObject
    {
        public World World { get; private set; }

        public void Initialise()
        {
            World.TypeRegistry.RegisterTypesFromAssemblyContaining<IAssemblyMarker>();
            World = new World();
        }
    }
}

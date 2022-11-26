using ECS.Public.Extensions;
using ECSExample.Scripts.Static;
using TMPro;
using UnityEngine;

namespace ECSExample.Scripts.Monobehaviours
{
    public class EntityCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text textBox;

        [SerializeField] private WorldProvider worldProvider;
        void Update()
        {
            var entityCount = worldProvider.World.EntityCount();
            textBox.text = $"Total entities: {entityCount.ToString()}";
        }
    }
}

using ECS.Public.Classes;
using ECSExample.Scripts.Static;

namespace ECSExample.Scripts.ECS.TypeDefinitions
{
    public class CubeType : TypeList
    {
        public CubeType() : base(
            Type<GameobjectComponent>,
            Type<PositionComponent>,
            Type<VelocityComponent>
            )
        { }
    }
}
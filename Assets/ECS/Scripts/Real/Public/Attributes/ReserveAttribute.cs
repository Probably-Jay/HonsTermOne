using System;

namespace ECS.Scripts.Real.Public.Attributes
{
    [AttributeUsage(AttributeTargets.Struct )]
    public class ReserveInComponentArray : Attribute
    {
        public ulong ReserveSize { get; }

        public ReserveInComponentArray(ulong reserveSize)
        {
            ReserveSize = reserveSize;
        }
    }
}
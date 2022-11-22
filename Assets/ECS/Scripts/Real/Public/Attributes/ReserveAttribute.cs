using System;

namespace ECS.Scripts.Real.Public.Attributes
{
    public class ReserveInComponentArray : Attribute
    {
        public ulong ReserveSize { get; }

        public ReserveInComponentArray(ulong reserveSize)
        {
            ReserveSize = reserveSize;
        }
    }
}
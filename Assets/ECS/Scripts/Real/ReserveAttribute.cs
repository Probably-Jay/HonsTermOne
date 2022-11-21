using System;

namespace ECS.Scripts.Real
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
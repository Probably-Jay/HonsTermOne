using System;

namespace ECS.Scripts.Real
{
    public class ReserveInComponentArray : Attribute
    {
        public uint ReserveSize { get; }

        public ReserveInComponentArray(uint reserveSize)
        {
            ReserveSize = reserveSize;
        }
    }
}
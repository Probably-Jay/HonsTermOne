using System;

namespace ECS.Public.Attributes
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
using System;
using JetBrains.Annotations;

namespace ECS.Public.Attributes
{
    [PublicAPI]
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
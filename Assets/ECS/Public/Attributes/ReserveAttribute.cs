using System;
using JetBrains.Annotations;

namespace ECS.Public.Attributes
{
    /// <summary>
    /// Indicates that space in the world's component array should be reserved
    /// </summary>
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
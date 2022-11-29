namespace ECS.Internal.Helper
{
    internal static class BitOperations
    {
        public static ulong RoundUpToPowerOf2(ulong val)
        {
            var x = 2u;
            while (x < val)
            {
                x *= 2;
            }

            return x;
        }
    }
}
namespace ECS.Internal.Helper
{
    internal static class BitOperations
    {
       // public static int RoundUpToPowerOf2(int val) => RoundUpToPowerOf2((ulong)val);

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
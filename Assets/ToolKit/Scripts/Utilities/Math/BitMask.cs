namespace Bini.ToolKit.Core.Unity.Utilities.Math
{
    public static class BitMask
    {
        public static ulong Make(params ulong[] bits)
        {
            ulong res = 0;
            var shift = 0;

            for (var i = bits.Length - 1; i >= 0; i--)
            {
                res |= bits[i] << shift;
                shift++;
            }

            return res;
        }
    }
}
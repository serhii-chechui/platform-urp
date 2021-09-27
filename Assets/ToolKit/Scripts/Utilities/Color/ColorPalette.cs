namespace Bini.ToolKit.Core.Unity.Utilities.Color
{
    public class ColorPalette
    {
        private const float Color4Factor = 15f;
        private const float Color5Factor = 31f;
        private const float Color6Factor = 63f;

        private readonly UnityEngine.Color[] palette;
        private readonly int paletteSize;

        public ColorPalette(UnityEngine.Color[] palette)
        {
            this.palette = palette;
            paletteSize = palette.Length;
        }

        public UnityEngine.Color FindRgb(UnityEngine.Color color)
        {
            var bestDist = 5f; // (1^2)*4 + 1
            var bestColor = color;
            for (var i = 0; i < paletteSize; i++)
            {
                var pc = palette[i];
                var dr = color.r - pc.r;
                var dg = color.g - pc.g;
                var db = color.b - pc.b;
                var dist = dr * dr + dg * dg + db * db;
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestColor = pc;
                }
            }

            return bestColor;
        }

        public UnityEngine.Color FindRgba(UnityEngine.Color color)
        {
            var bestDist = 5f; // (1^2)*4 + 1
            var bestColor = color;
            for (var i = 0; i < paletteSize; i++)
            {
                var pc = palette[i];
                var dr = color.r - pc.r;
                var dg = color.g - pc.g;
                var db = color.b - pc.b;
                var da = color.b - pc.b;
                var dist = dr * dr + dg * dg + db * db + da * da;
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestColor = pc;
                }
            }

            return bestColor;
        }

        public static UnityEngine.Color Rgb565(UnityEngine.Color color)
        {
            color.r = (int)(color.r * Color5Factor + 0.5f) / Color5Factor;
            color.g = (int)(color.g * Color6Factor + 0.5f) / Color6Factor;
            color.b = (int)(color.b * Color5Factor + 0.5f) / Color5Factor;
            color.a = 1f;
            return color;
        }

        public static UnityEngine.Color Rgba4444(UnityEngine.Color color)
        {
            color.r = (int)(color.r * Color4Factor + 0.5f) / Color4Factor;
            color.g = (int)(color.g * Color4Factor + 0.5f) / Color4Factor;
            color.b = (int)(color.b * Color4Factor + 0.5f) / Color4Factor;
            color.a = (int)(color.a * Color4Factor + 0.5f) / Color4Factor;
            return color;
        }
    }
}
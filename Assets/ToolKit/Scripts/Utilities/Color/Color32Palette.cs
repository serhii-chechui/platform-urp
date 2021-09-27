using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Utilities.Color
{
    public class Color32Palette
    {
        private const int BestDistance = 260101; // (255^2)*4 + 1
        private const float Color4Factor = 15f / 255f;
        private const float Color5Factor = 31f / 255f;
        private const float Color6Factor = 63f / 255f;

        private readonly Color32[] palette;
        private readonly int paletteSize;

        public Color32Palette(Color32[] palette)
        {
            this.palette = palette;
            paletteSize = palette.Length;
        }

        public Color32 FindRgb(Color32 color)
        {
            var bestDist = BestDistance;
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

        public Color32 FindRgba(Color32 color)
        {
            var bestDist = BestDistance;
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

        public static Color32 Rgb565(Color32 color)
        {
            color.r = (byte)((int)(color.r * Color5Factor + 0.5f) / Color5Factor + 0.5f);
            color.g = (byte)((int)(color.g * Color6Factor + 0.5f) / Color6Factor + 0.5f);
            color.b = (byte)((int)(color.b * Color5Factor + 0.5f) / Color5Factor + 0.5f);
            color.a = 255;
            return color;
        }

        public static Color32 Rgba4444(Color32 color)
        {
            color.r = (byte)((int)(color.r * Color4Factor + 0.5f) / Color4Factor + 0.5f);
            color.g = (byte)((int)(color.g * Color4Factor + 0.5f) / Color4Factor + 0.5f);
            color.b = (byte)((int)(color.b * Color4Factor + 0.5f) / Color4Factor + 0.5f);
            color.a = (byte)((int)(color.a * Color4Factor + 0.5f) / Color4Factor + 0.5f);
            return color;
        }
    }
}
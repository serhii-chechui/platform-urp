using Bini.ToolKit.Core.Unity.Utilities.Color;
using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Extensions
{
    public static class ColorExtensions
    {
        public static UnityEngine.Color Add(this UnityEngine.Color color1, UnityEngine.Color color2)
        {
            return new UnityEngine.Color(color1.r + color2.r,
                color1.g + color2.g,
                color1.b + color2.b,
                color1.a + color2.a);
        }

        public static Color32 Add(this Color32 color1, Color32 color2)
        {
            return new Color32((byte)(color1.r + color2.r),
                (byte)(color1.g + color2.g),
                (byte)(color1.b + color2.b),
                (byte)(color1.a + color2.a));
        }

        public static UnityEngine.Color Grayscale(this UnityEngine.Color color, GrayscaleMode mode = GrayscaleMode.Average)
        {
            var v = ColorHelper.Grayscale(color.r, color.g, color.b, mode);
            return new UnityEngine.Color(v, v, v, color.a);
        }

        public static Color32 Grayscale(this Color32 color, GrayscaleMode mode = GrayscaleMode.Average)
        {
            var v = (byte)(ColorHelper.Grayscale(color.r / 255f, color.g / 255f, color.b / 255f, mode) * 255);
            return new Color32(v, v, v, color.a);
        }

        public static UnityEngine.Color Multiply(this UnityEngine.Color color, UnityEngine.Color factor)
        {
            return new UnityEngine.Color(color.r * factor.r, color.g * factor.g, color.b * factor.b, color.a * factor.a);
        }

        public static Color32 Multiply(this Color32 color, Color32 factor)
        {
            return new Color32((byte)(color.r * factor.r / 255),
                (byte)(color.g * factor.g / 255),
                (byte)(color.b * factor.b / 255),
                (byte)(color.a * factor.a / 255));
        }

        public static UnityEngine.Color Multiply(this UnityEngine.Color color, float factor)
        {
            return new UnityEngine.Color(color.r * factor, color.g * factor, color.b * factor, color.a * factor);
        }

        public static Color32 Multiply(this Color32 color, float factor)
        {
            return new Color32((byte)(color.r * factor),
                (byte)(color.g * factor),
                (byte)(color.b * factor),
                (byte)(color.a * factor));
        }

        public static Color32 Multiply(this Color32 color, int numerator, int denominator)
        {
            return new Color32((byte)(color.r * numerator / denominator),
                (byte)(color.g * numerator / denominator),
                (byte)(color.b * numerator / denominator),
                (byte)(color.a * numerator / denominator));
        }

        public static bool SameAs(this UnityEngine.Color color0, ref UnityEngine.Color color1)
        {
            return color0.r == color1.r && color0.g == color1.g && color0.b == color1.b && color0.a == color1.a;
        }

        public static bool SameAs(this Color32 color0, ref Color32 color1)
        {
            return color0.r == color1.r && color0.g == color1.g && color0.b == color1.b && color0.a == color1.a;
        }

        public static bool SameAs(this UnityEngine.Color color0, UnityEngine.Color color1)
        {
            return color0.r == color1.r && color0.g == color1.g && color0.b == color1.b && color0.a == color1.a;
        }

        public static bool SameAs(this Color32 color0, Color32 color1)
        {
            return color0.r == color1.r && color0.g == color1.g && color0.b == color1.b && color0.a == color1.a;
        }

        public static UnityEngine.Color SetA(this UnityEngine.Color color, float value)
        {
            color.a = value;
            return color;
        }

        public static Color32 SetA(this Color32 color, byte value)
        {
            color.a = value;
            return color;
        }

        public static UnityEngine.Color SetB(this UnityEngine.Color color, float value)
        {
            color.b = value;
            return color;
        }

        public static Color32 SetB(this Color32 color, byte value)
        {
            color.b = value;
            return color;
        }

        public static UnityEngine.Color SetG(this UnityEngine.Color color, float value)
        {
            color.g = value;
            return color;
        }

        public static Color32 SetG(this Color32 color, byte value)
        {
            color.g = value;
            return color;
        }

        public static UnityEngine.Color SetR(this UnityEngine.Color color, float value)
        {
            color.r = value;
            return color;
        }

        public static Color32 SetR(this Color32 color, byte value)
        {
            color.r = value;
            return color;
        }

        public static UnityEngine.Color Subtract(this UnityEngine.Color color1, UnityEngine.Color color0)
        {
            return new UnityEngine.Color(color1.r - color0.r, color1.g - color0.g, color1.b - color0.b, color1.a - color0.a);
        }

        public static Color32 Subtract(this Color32 color1, Color32 color0)
        {
            return new Color32((byte)(color1.r - color0.r),
                (byte)(color1.g - color0.g),
                (byte)(color1.b - color0.b),
                (byte)(color1.a - color0.a));
        }
    }
}
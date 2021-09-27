using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Utilities.Color
{
    public class ColorHelper
    {
        public static UnityEngine.Color Average(params UnityEngine.Color[] colors)
        {
            float r = 0f, g = 0f, b = 0f, a = 0f;
            for (var i = 0; i < colors.Length; i++)
            {
                var c = colors[i];
                r += c.r;
                g += c.g;
                b += c.b;
                a += c.a;
            }

            float n = colors.Length;
            return new UnityEngine.Color(r / n, g / n, b / n, a / n);
        }

        public static Color32 Average(params Color32[] colors)
        {
            int r = 0, g = 0, b = 0, a = 0;
            for (var i = 0; i < colors.Length; i++)
            {
                var c = colors[i];
                r += c.r;
                g += c.g;
                b += c.b;
                a += c.a;
            }

            double n = colors.Length;
            return new Color32((byte)(r / n), (byte)(g / n), (byte)(b / n), (byte)(a / n));
        }

        public static Color32 FromRgba(int value)
        {
            var r = (byte)(value & 0xff);
            var g = (byte)((value >> 8) & 0xff);
            var b = (byte)((value >> 16) & 0xff);
            var a = (byte)((value >> 24) & 0xff);
            return new Color32(r, g, b, a);
        }

        public static float Grayscale(float r, float g, float b, GrayscaleMode mode = GrayscaleMode.Average)
        {
            switch (mode)
            {
                case GrayscaleMode.Lightness:
                    // http://docs.gimp.org/en/gimp-tool-desaturate.html
                    return (Mathf.Max(r, g, b) + Mathf.Min(r, g, b)) / 2f;
                case GrayscaleMode.Luminosity:
                    // http://docs.gimp.org/en/gimp-tool-desaturate.html
                    return 0.21f * r + 0.72f * g + 0.07f * b;
                case GrayscaleMode.Average:
                    // http://docs.gimp.org/en/gimp-tool-desaturate.html
                    return (r + g + b) / 3f;
                case GrayscaleMode.Perceived:
                    // http://alienryderflex.com/hsp.html
                    return Mathf.Sqrt(0.241f * r * r + 0.691f * g * g + 0.068f * b * b);
                case GrayscaleMode.W3C:
                    // http://www.w3.org/TR/AERT#color-contrast
                    return r * 0.299f + g * 0.587f + b * 0.114f;
                default:
                    return (r + g + b) / 3f;
            }
        }

        public static UnityEngine.Color Max(params UnityEngine.Color[] colors)
        {
            var c0 = colors[0];
            for (var i = 1; i < colors.Length; i++)
            {
                var c = colors[i];
                if (c.r > c0.r)
                    c0.r = c.r;

                if (c.g > c0.g)
                    c0.g = c.g;

                if (c.b > c0.b)
                    c0.b = c.b;

                if (c.a > c0.a)
                    c0.a = c.a;
            }

            return c0;
        }

        public static Color32 Max(params Color32[] colors)
        {
            var c0 = colors[0];
            for (var i = 1; i < colors.Length; i++)
            {
                var c = colors[i];
                if (c.r > c0.r)
                    c0.r = c.r;

                if (c.g > c0.g)
                    c0.g = c.g;

                if (c.b > c0.b)
                    c0.b = c.b;

                if (c.a > c0.a)
                    c0.a = c.a;
            }

            return c0;
        }

        public static UnityEngine.Color Min(params UnityEngine.Color[] colors)
        {
            var c0 = colors[0];
            for (var i = 1; i < colors.Length; i++)
            {
                var c = colors[i];
                if (c.r < c0.r)
                    c0.r = c.r;

                if (c.g < c0.g)
                    c0.g = c.g;

                if (c.b < c0.b)
                    c0.b = c.b;

                if (c.a < c0.a)
                    c0.a = c.a;
            }

            return c0;
        }

        public static Color32 Min(params Color32[] colors)
        {
            var c0 = colors[0];
            for (var i = 1; i < colors.Length; i++)
            {
                var c = colors[i];
                if (c.r < c0.r)
                    c0.r = c.r;

                if (c.g < c0.g)
                    c0.g = c.g;

                if (c.b < c0.b)
                    c0.b = c.b;

                if (c.a < c0.a)
                    c0.a = c.a;
            }

            return c0;
        }

        public static UnityEngine.Color Sum(params UnityEngine.Color[] colors)
        {
            float r = 0f, g = 0f, b = 0f, a = 0f;
            for (var i = 0; i < colors.Length; i++)
            {
                var c = colors[i];
                r += c.r;
                g += c.g;
                b += c.b;
                a += c.a;
            }

            return new UnityEngine.Color(r, g, b, a);
        }

        public static Color32 Sum(params Color32[] colors)
        {
            int r = 0, g = 0, b = 0, a = 0;
            for (var i = 0; i < colors.Length; i++)
            {
                var c = colors[i];
                r += c.r;
                g += c.g;
                b += c.b;
                a += c.a;
            }

            return new Color32((byte)r, (byte)g, (byte)b, (byte)a);
        }

        public static int ToRgba(Color32 color)
        {
            return color.r | (color.g << 8) | (color.b << 16) | (color.a << 24);
        }
    }
}
using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Utilities.Color
{
    public struct ColorHsv
    {
        public float Hue;
        public float Saturation;
        public float Value;
        public float Alpha;

        public ColorHsv(float hue, float saturation, float value, float alpha = 1f)
        {
            Hue = hue;
            Saturation = saturation;
            Value = value;
            Alpha = alpha;
        }

        public void Sanitize()
        {
            Hue = (Hue % 1f + 1f) % 1f;
            Saturation = Mathf.Clamp01(Saturation);
            Value = Mathf.Clamp01(Value);
            Alpha = Mathf.Clamp01(Alpha);
        }

        public static implicit operator ColorHsv(UnityEngine.Color color)
        {
            var minVal = Mathf.Min(Mathf.Min(color.r, color.g), color.b);
            var maxVal = Mathf.Max(Mathf.Max(color.r, color.g), color.b);
            var delta = maxVal - minVal;

            var v = maxVal;

            if (delta == 0f)
                return new ColorHsv(0f, 0f, v, color.a);

            var s = delta / maxVal;

            float h;
            if (color.r == maxVal)
                h = (color.g - color.b) / (6f * delta);
            else if (color.g == maxVal)
                h = 1f / 3f + (color.b - color.r) / (6f * delta);
            else
                h = 2f / 3f + (color.r - color.g) / (6f * delta);

            h = (h % 1f + 1f) % 1f;

            return new ColorHsv(h, s, v, color.a);
        }

        public static implicit operator ColorHsv(Color32 color)
        {
            return (UnityEngine.Color)color;
        }

        public static implicit operator UnityEngine.Color(ColorHsv color)
        {
            var s = Mathf.Clamp01(color.Saturation);
            var v = Mathf.Clamp01(color.Value);
            if (s == 0f)
                return new UnityEngine.Color(v, v, v, color.Alpha);

            var h = (color.Hue % 1f + 1f) % 1f;
            var varH = h * 6f;
            var varI = Mathf.FloorToInt(varH);
            var var1 = v * (1f - s);
            var var2 = v * (1f - s * (varH - varI));
            var var3 = v * (1f - s * (1f - (varH - varI)));

            switch (varI)
            {
                case 1:
                    return new UnityEngine.Color(var2, v, var1, color.Alpha);
                case 2:
                    return new UnityEngine.Color(var1, v, var3, color.Alpha);
                case 3:
                    return new UnityEngine.Color(var1, var2, v, color.Alpha);
                case 4:
                    return new UnityEngine.Color(var3, var1, v, color.Alpha);
                case 5:
                    return new UnityEngine.Color(v, var1, var2, color.Alpha);
                default:
                    return new UnityEngine.Color(v, var3, var1, color.Alpha); // 0 or 6
            }
        }

        public static implicit operator Color32(ColorHsv color)
        {
            return (UnityEngine.Color)color;
        }
    }
}
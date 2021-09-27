using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Utilities.Math
{
    public static class MathUtils
    {
        private static readonly Vector4 rgbaDecodeFloat = new Vector4(1f, 1f / 255f, 1f / 65025f, 1 / 16581375f);

        private static readonly Vector4 rgbaEncodeFloat = new Vector4(1f, 255f, 65025f, 16581375f);

        public static float DistanceToSegment(Vector3 start, Vector3 end, Vector3 point)
        {
            var dv = end - start;
            var dvSize = dv.magnitude;

            if (dvSize <= Mathf.Epsilon)
                return Vector3.Distance(start, point);

            var n = dv * (1f / dvSize);
            var dp = point - start;
            var proj = Mathf.Clamp(Vector3.Dot(dp, n), 0f, dvSize);
            return (dp - n * proj).magnitude;
        }

        // https://ideone.com/PnPJgb
        // https://github.com/pgkelley4/line-segments-intersect/blob/master/js/line-segments-intersect.js
        public static bool LinesIntersect(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
        {
            var CmP = new Vector2(start2.x - start1.x, start2.y - start1.y);
            var r = new Vector2(end1.x - start1.x, end1.y - start1.y);
            var s = new Vector2(end2.x - start2.x, end2.y - start2.y);

            var CmPxr = CmP.x * r.y - CmP.y * r.x;
            var CmPxs = CmP.x * s.y - CmP.y * s.x;
            var rxs = r.x * s.y - r.y * s.x;

            if (CmPxr == 0f) // Lines are collinear, and so intersect if they have any overlap
                return start2.x - start1.x < 0f != start2.x - end1.x < 0f || start2.y - start1.y < 0f != start2.y - end1.y < 0f;

            if (rxs == 0f)
                return false; // Lines are parallel.

            var rxsr = 1f / rxs;
            var t = CmPxs * rxsr;
            var u = CmPxr * rxsr;

            return t >= 0f && t <= 1f && u >= 0f && u <= 1f;
        }

        // https://www.redblobgames.com/articles/visibility/Visibility.hx
        // NOT TESTED
        public static Vector2 LineLineIntersection(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
        {
            var s = ((end2.x - start2.x) * (start1.y - start2.y) - (end2.y - start2.y) * (start1.x - start2.x))
                    / ((end2.y - start2.y) * (end1.x - start1.x) - (end2.x - start2.x) * (end1.y - start1.y));

            return new Vector2(start1.x + s * (end1.x - start1.x), start1.y + s * (end1.y - start1.y));
        }

        public static Vector4 EncodeFloatRgba(float value)
        {
            const float _255 = 1f / 255f;
            var enc = rgbaEncodeFloat * Mathf.Clamp01(value);
            enc.y = enc.y % 1f;
            enc.z = enc.z % 1f;
            enc.w = enc.w % 1f;
            enc.x -= enc.y * _255;
            enc.y -= enc.z * _255;
            enc.z -= enc.w * _255;
            return enc;
        }

        public static float DecodeFloatRgba(Vector4 rgba)
        {
            return Vector4.Dot(rgba, rgbaDecodeFloat);
        }

        public static void Swap<T>(ref T value1, ref T value2)
        {
            var wrk = value1;
            value1 = value2;
            value2 = wrk;
        }
    }
}
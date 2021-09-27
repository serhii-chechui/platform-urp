using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Extensions
{
    public static class VectorExtensions
    {
        public static Vector2 Absolute(this Vector2 vector)
        {
            return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
        }

        public static Vector3 Absolute(this Vector3 vector)
        {
            return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        }

        public static Vector4 Absolute(this Vector4 vector)
        {
            return new Vector4(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z), Mathf.Abs(vector.w));
        }

        public static Vector2 Divide(this Vector2 vector, float factor)
        {
            return new Vector2(vector.x / factor, vector.y / factor);
        }

        public static Vector3 Divide(this Vector3 vector, float factor)
        {
            return new Vector3(vector.x / factor, vector.y / factor, vector.z / factor);
        }

        public static Vector4 Divide(this Vector4 vector, float factor)
        {
            return new Vector4(vector.x / factor, vector.y / factor, vector.z / factor, vector.w / factor);
        }

        public static Vector2 Divide(this Vector2 vector, Vector2 factor)
        {
            return new Vector2(vector.x / factor.x, vector.y / factor.y);
        }

        public static Vector3 Divide(this Vector3 vector, Vector3 factor)
        {
            return new Vector3(vector.x / factor.x, vector.y / factor.y, vector.z / factor.z);
        }

        public static Vector4 Divide(this Vector4 vector, Vector4 factor)
        {
            return new Vector4(vector.x / factor.x, vector.y / factor.y, vector.z / factor.z, vector.w / factor.w);
        }

        public static Rect Encapsulate(this Rect rect, Vector2 point)
        {
            return rect.Encapsulate(point.x, point.y);
        }

        public static Rect Encapsulate(this Rect rect, float x, float y)
        {
            rect.xMin = Mathf.Min(rect.xMin, x);
            rect.xMax = Mathf.Max(rect.xMax, x);
            rect.yMin = Mathf.Min(rect.yMin, y);
            rect.yMax = Mathf.Max(rect.yMax, y);
            return rect;
        }

        public static Rect Encapsulate(this Rect rect, Rect other)
        {
            rect.xMin = Mathf.Min(rect.xMin, other.xMin);
            rect.xMax = Mathf.Max(rect.xMax, other.xMax);
            rect.yMin = Mathf.Min(rect.yMin, other.yMin);
            rect.yMax = Mathf.Max(rect.yMax, other.yMax);
            return rect;
        }

        public static Rect Expand(this Rect rect, Vector2 value)
        {
            return rect.Expand(value.x, value.y);
        }

        public static Rect Expand(this Rect rect, float value)
        {
            return rect.Expand(value, value);
        }

        public static Rect Expand(this Rect rect, float x, float y)
        {
            rect.xMin -= x;
            rect.yMin -= y;
            rect.xMax += x;
            rect.yMax += y;
            return rect;
        }

        public static Vector2 Extents(this Rect rect)
        {
            return new Vector2(rect.width * 0.5f, rect.height * 0.5f);
        }

        public static Vector3 ExtractPosition(this Matrix4x4 matrix)
        {
            return matrix.GetColumn(3);
        }

        public static Quaternion ExtractRotation(this Matrix4x4 matrix)
        {
            return Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));
        }

        public static Vector3 ExtractScale(this Matrix4x4 matrix)
        {
            return new Vector3(matrix.GetColumn(0).magnitude,
                matrix.GetColumn(1).magnitude,
                matrix.GetColumn(2).magnitude);
        }

        public static Vector2 GetVertex(this Rect rect, int indexX, int indexY)
        {
            return new Vector2(indexX > 0 ? rect.xMax : rect.xMin, indexY > 0 ? rect.yMax : rect.yMin);
        }

        public static Vector3 GetVertex(this Bounds bounds, int indexX, int indexY, int indexZ)
        {
            Vector3 min = bounds.min, max = bounds.max;
            return new Vector3(indexX > 0 ? max.x : min.x, indexY > 0 ? max.y : min.y, indexZ > 0 ? max.z : min.z);
        }

        public static Vector3 InverseTransformDirection(this Transform transform, Vector3 vector, Transform other)
        {
            if (other != null)
                vector = other.localToWorldMatrix.MultiplyVector(vector);

            return transform.worldToLocalMatrix.MultiplyVector(vector);
        }

        public static Vector3 InverseTransformPoint(this Transform transform, Vector3 vector, Transform other)
        {
            if (other)
                vector = other.localToWorldMatrix.MultiplyPoint3x4(vector);

            return transform.worldToLocalMatrix.MultiplyPoint3x4(vector);
        }

        public static float Max(this Vector2 vector)
        {
            return Mathf.Max(vector.x, vector.y);
        }

        public static float Max(this Vector3 vector)
        {
            return Mathf.Max(Mathf.Max(vector.x, vector.y), vector.z);
        }

        public static float Max(this Vector4 vector)
        {
            return Mathf.Max(Mathf.Max(vector.x, vector.y), Mathf.Max(vector.z, vector.w));
        }

        public static Vector2 Max(this Rect rect)
        {
            return new Vector2(rect.xMax, rect.yMax);
        }

        public static float Min(this Vector2 vector)
        {
            return Mathf.Min(vector.x, vector.y);
        }

        public static float Min(this Vector3 vector)
        {
            return Mathf.Min(Mathf.Min(vector.x, vector.y), vector.z);
        }

        public static float Min(this Vector4 vector)
        {
            return Mathf.Min(Mathf.Min(vector.x, vector.y), Mathf.Min(vector.z, vector.w));
        }

        public static Vector2 Min(this Rect rect)
        {
            return new Vector2(rect.xMin, rect.yMin);
        }

        public static Vector2 Multiply(this Vector2 vector, float factor)
        {
            return new Vector2(vector.x * factor, vector.y * factor);
        }

        public static Vector3 Multiply(this Vector3 vector, float factor)
        {
            return new Vector3(vector.x * factor, vector.y * factor, vector.z * factor);
        }

        public static Vector4 Multiply(this Vector4 vector, float factor)
        {
            return new Vector4(vector.x * factor, vector.y * factor, vector.z * factor, vector.w * factor);
        }

        public static Vector2 Multiply(this Vector2 vector, Vector2 factor)
        {
            return new Vector2(vector.x * factor.x, vector.y * factor.y);
        }

        public static Vector3 Multiply(this Vector3 vector, Vector3 factor)
        {
            return new Vector3(vector.x * factor.x, vector.y * factor.y, vector.z * factor.z);
        }

        public static Vector4 Multiply(this Vector4 vector, Vector4 factor)
        {
            return new Vector4(vector.x * factor.x, vector.y * factor.y, vector.z * factor.z, vector.w * factor.w);
        }

        public static Vector2 Size(this Rect rect)
        {
            return new Vector2(rect.width, rect.height);
        }

        public static Vector2 To2D(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static Vector2 To2D(this Vector4 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static Vector3 To3D(this Vector2 vector)
        {
            return new Vector3(vector.x, vector.y);
        }

        public static Vector3 To3D(this Vector2 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        public static Vector3 To3D(this Vector4 vector)
        {
            return new Vector3(vector.x, vector.y, vector.z);
        }

        public static Vector4 To4D(this Vector2 vector)
        {
            return new Vector4(vector.x, vector.y);
        }

        public static Vector4 To4D(this Vector2 vector, float z)
        {
            return new Vector4(vector.x, vector.y, z);
        }

        public static Vector4 To4D(this Vector2 vector, float z, float w)
        {
            return new Vector4(vector.x, vector.y, z, w);
        }

        public static Vector4 To4D(this Vector3 vector)
        {
            return new Vector4(vector.x, vector.y, vector.z);
        }

        public static Vector4 To4D(this Vector3 vector, float w)
        {
            return new Vector4(vector.x, vector.y, vector.z, w);
        }

        public static Bounds ToBounds(this Rect rect, float centerZ = 0f, float sizeZ = 0f)
        {
            return new Bounds(rect.center.To3D(centerZ), rect.Size().To3D(sizeZ));
        }

        public static Rect ToRect(this Bounds bounds)
        {
            return new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y);
        }

        public static Vector3 TransformDirection(this Transform transform, Vector3 vector, Transform other)
        {
            vector = transform.localToWorldMatrix.MultiplyVector(vector);

            if (other == null)
                return vector;

            return other.worldToLocalMatrix.MultiplyVector(vector);
        }

        public static Vector3 TransformPoint(this Transform transform, Vector3 vector, Transform other)
        {
            vector = transform.localToWorldMatrix.MultiplyPoint3x4(vector);

            if (other == null)
                return vector;

            return other.worldToLocalMatrix.MultiplyPoint3x4(vector);
        }
    }
}
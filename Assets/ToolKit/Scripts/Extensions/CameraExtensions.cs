using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Extensions
{
    public static class CameraExtensions
    {
        /// <summary>
        /// Returns the size (width, height) of a camera's pixel in the local space
        /// of the specified transform (or in the world space) at the specified depth (z).
        /// </summary>
        public static Vector2 GetPixelSize(this Camera camera, Transform transform = null, float z = 0)
        {
            var origin = camera.ViewportToWorldPoint(new Vector3(0, 0, z));
            var axisX = camera.ViewportToWorldPoint(new Vector3(1f / camera.pixelWidth, 0, z));
            var axisY = camera.ViewportToWorldPoint(new Vector3(0, 1f / camera.pixelHeight, z));

            if (transform.IsAlive())
            {
                var matrix = transform.worldToLocalMatrix;
                origin = matrix.MultiplyPoint3x4(origin);
                axisX = matrix.MultiplyPoint3x4(axisX);
                axisY = matrix.MultiplyPoint3x4(axisY);
            }

            return new Vector2((axisX - origin).magnitude, (axisY - origin).magnitude);
        }
    }
}
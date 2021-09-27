using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Extensions
{
    public static class PhysicsExtensions
    {
        // https://feedback.unity3d.com/suggestions/collider2d-dot-raycast-simple-enough-why-cant-i-find-it
        /// <summary>
        ///     Raycast the against the collider itself (equivalent of Collider.Raycast)
        /// </summary>
        public static bool Raycast(this Collider2D collider, Ray2D ray, out RaycastHit2D hitInfo, float maxDistance)
        {
            var originalLayer = collider.gameObject.layer;
            const int tempLayer = 31;
            collider.gameObject.layer = tempLayer;
            hitInfo = Physics2D.Raycast(ray.origin, ray.direction, maxDistance, 1 << tempLayer);
            collider.gameObject.layer = originalLayer;

            if (hitInfo.collider && hitInfo.collider != collider)
            {
                Debug.LogError("Collider2D.Raycast() need a unique temp layer to work! Make sure Layer #" + tempLayer +
                               " is unused!");

                return false;
            }

            return hitInfo.collider != null;
        }
    }
}
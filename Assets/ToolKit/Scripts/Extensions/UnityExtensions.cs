using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Extensions
{
    public static class UnityExtensions
    {
        /// <summary>
        /// Returns true if the object is not null and not destroyed
        /// </summary>
        public static bool IsAlive(this Object @object)
        {
            return @object;
        }

        /// <summary>
        /// Returns true if the object is null or destroyed
        /// </summary>
        public static bool IsNotAlive(this Object @object)
        {
            return !@object;
        }

        /// <summary>
        /// Safely disposes of the object, taking into account play/edit mode and other specifics.
        /// Returns null, for convenience.
        /// </summary>
        public static T Dispose<T>(this T @object, bool destroyAssets = false) where T : Object
        {
            if (!@object.IsAlive()) return null;

            // RenderTextures require special handling
            if (@object is RenderTexture renderTexture)
            {
                if (RenderTexture.active == renderTexture)
                    RenderTexture.active = null;

                if ((renderTexture.hideFlags & HideFlags.HideAndDontSave) != 0)
                {
                    // DO NOT release/destroy temporary RenderTextures!
                    // (Unity handles their destruction separately)
                    RenderTexture.ReleaseTemporary(renderTexture);
                    return null;
                }

                renderTexture.Release();
            }

#if UNITY_EDITOR
            if (Application.isPlaying)
                Object.Destroy(@object);
            else
                Object.DestroyImmediate(@object, destroyAssets);
#else
			Object.Destroy(@object);
#endif

            return null;
        }
    }
}
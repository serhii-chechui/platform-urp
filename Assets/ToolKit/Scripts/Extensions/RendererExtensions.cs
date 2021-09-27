using Bini.ToolKit.Core.Unity.Utilities.Mesh;
using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Extensions
{
    public static class RendererExtensions
    {
        public static void UpdateBounds(this SkinnedMeshRenderer skin, Matrix4x4 matrix, ref Bounds bounds,
            ref int count, bool useVertices = true, int maxBones = -1)
        {
            var vIter = useVertices
                ? SkinnedMeshHelper.IterateSkinnedVertices(skin, matrix, maxBones)
                : MeshUtils.IterateTransformedBoundingBoxCorners(skin, matrix);

            foreach (var v in vIter)
            {
                if (count == 0)
                    bounds = new Bounds(v, Vector3.zero);
                else
                    bounds.Encapsulate(v);

                ++count;
            }
        }

        public static void UpdateBounds(this MeshFilter meshFilter, Matrix4x4 matrix, ref Bounds bounds, ref int count,
            bool useVertices = true)
        {
            var vIter = useVertices
                ? MeshUtils.IterateTransformedVertices(meshFilter, matrix)
                : MeshUtils.IterateTransformedBoundingBoxCorners(meshFilter, matrix);

            foreach (var v in vIter)
            {
                if (count == 0)
                    bounds = new Bounds(v, Vector3.zero);
                else
                    bounds.Encapsulate(v);

                ++count;
            }
        }

        public static void UpdateBounds(this SpriteRenderer spriteRenderer, Matrix4x4 matrix, ref Bounds bounds,
            ref int count, bool useVertices = true)
        {
            var vIter = useVertices
                ? MeshUtils.IterateTransformedVertices(spriteRenderer, matrix)
                : MeshUtils.IterateTransformedBoundingBoxCorners(spriteRenderer, matrix);

            foreach (var v in vIter)
            {
                if (count == 0)
                    bounds = new Bounds(v, Vector3.zero);
                else
                    bounds.Encapsulate(v);

                ++count;
            }
        }
    }
}
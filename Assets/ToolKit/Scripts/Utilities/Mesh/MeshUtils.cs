using System.Collections.Generic;
using Bini.ToolKit.Core.Unity.Extensions;
using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Utilities.Mesh
{
    public static class MeshUtils
    {
        public static Bounds CalculateBounds(Vector3[] vertices)
        {
            if (vertices.Length == 0)
                return new Bounds(Vector3.zero, Vector3.zero);

            var bounds = new Bounds(vertices[0], Vector3.zero);

            for (var i = 1; i < vertices.Length; i++)
                bounds.Encapsulate(vertices[i]);

            return bounds;
        }

        public static Bounds CalculateBounds(UnityEngine.Mesh mesh)
        {
            return CalculateBounds(mesh.vertices);
        }

        public static Rect CalculateUVBounds(Vector2[] uv)
        {
            if (uv.Length == 0)
                return new Rect(0, 0, 0, 0);

            var bounds = new Rect(uv[0].x, uv[0].y, 0, 0);

            for (var i = 1; i < uv.Length; i++)
            {
                var p = uv[i];
                bounds.xMin = Mathf.Min(bounds.xMin, p.x);
                bounds.xMax = Mathf.Max(bounds.xMax, p.x);
                bounds.yMin = Mathf.Min(bounds.yMin, p.y);
                bounds.yMax = Mathf.Max(bounds.yMax, p.y);
            }

            return bounds;
        }

        public static Rect CalculateUVBounds(UnityEngine.Mesh mesh, int uvSet = 0)
        {
            Vector2[] uv;

            switch (uvSet)
            {
                case 0:
                    uv = mesh.uv;
                    break;
                case 1:
                    uv = mesh.uv2;
                    break;
                case 2:
                    uv = mesh.uv3;
                    break;
                case 3:
                    uv = mesh.uv4;
                    break;
                default:
                    return new Rect(0, 0, 0, 0);
            }

            return CalculateUVBounds(uv);
        }

        public static void CopyMesh(UnityEngine.Mesh source, UnityEngine.Mesh destination)
        {
            destination.Clear();
            destination.vertices = source.vertices;
            destination.bounds = source.bounds;
            destination.normals = source.normals;
            destination.tangents = source.tangents;
            destination.boneWeights = source.boneWeights;
            destination.bindposes = source.bindposes;
            destination.colors32 = source.colors32;
            destination.uv = source.uv;
            destination.uv2 = source.uv2;
            destination.uv3 = source.uv3;
            destination.uv4 = source.uv4;
            destination.triangles = source.triangles;

            for (var i = 0; i < source.subMeshCount; i++)
                destination.SetIndices(source.GetIndices(i), source.GetTopology(i), i);
        }

        public static Rect FullUVRect(UnityEngine.Mesh mesh)
        {
            if (!mesh)
                return new Rect();

            var bounds = CalculateBounds(mesh);
            var uvBounds = CalculateUVBounds(mesh);

            var uv0 = uvBounds.Min();
            var uv1 = uvBounds.Max();
            var uvD = uv1 - uv0;

            if (uvD.Min() <= float.Epsilon)
                return new Rect();

            var p0 = bounds.min.To2D();
            var p1 = bounds.max.To2D();
            var pD = p1 - p0;

            var pDUVD = pD.Divide(uvD);
            p0 += Vector2.Scale(Vector2.zero - uv0, pDUVD);
            p1 += Vector2.Scale(Vector2.one - uv1, pDUVD);
            return new Rect(p0.x, p0.y, p1.x - p0.x, p1.y - p0.y);
        }

        public static IEnumerable<Vector3> IterateTransformedBoundingBoxCorners(SkinnedMeshRenderer skin, Matrix4x4 matrix)
        {
            return IterateTransformedBoundingBoxCorners(skin.localBounds, matrix * skin.transform.localToWorldMatrix);
        }

        public static IEnumerable<Vector3> IterateTransformedBoundingBoxCorners(MeshFilter meshFilter, Matrix4x4 matrix)
        {
            return IterateTransformedBoundingBoxCorners(meshFilter.sharedMesh.bounds,
                matrix * meshFilter.transform.localToWorldMatrix);
        }

        public static IEnumerable<Vector3> IterateTransformedBoundingBoxCorners(SpriteRenderer spriteRenderer, Matrix4x4 matrix)
        {
            return IterateTransformedBoundingBoxCorners(spriteRenderer.sprite.bounds,
                matrix * spriteRenderer.transform.localToWorldMatrix);
        }

        public static IEnumerable<Vector3> IterateTransformedBoundingBoxCorners(Bounds bounds, Matrix4x4 matrix)
        {
            Vector3 min = bounds.min, max = bounds.max;
            yield return matrix.MultiplyPoint3x4(new Vector3(min.x, min.y, min.z));
            yield return matrix.MultiplyPoint3x4(new Vector3(min.x, min.y, max.z));
            yield return matrix.MultiplyPoint3x4(new Vector3(min.x, max.y, min.z));
            yield return matrix.MultiplyPoint3x4(new Vector3(min.x, max.y, max.z));
            yield return matrix.MultiplyPoint3x4(new Vector3(max.x, min.y, min.z));
            yield return matrix.MultiplyPoint3x4(new Vector3(max.x, min.y, max.z));
            yield return matrix.MultiplyPoint3x4(new Vector3(max.x, max.y, min.z));
            yield return matrix.MultiplyPoint3x4(new Vector3(max.x, max.y, max.z));
        }

        public static IEnumerable<Vector3> IterateTransformedBoundingBoxCorners(RectTransform rectTransform, Matrix4x4 matrix)
        {
            return IterateTransformedBoundingBoxCorners(rectTransform.rect, matrix * rectTransform.localToWorldMatrix);
        }

        public static IEnumerable<Vector3> IterateTransformedBoundingBoxCorners(Rect rect, Matrix4x4 matrix)
        {
            Vector2 min = rect.min, max = rect.max;
            yield return matrix.MultiplyPoint3x4(new Vector3(min.x, min.y, 0));
            yield return matrix.MultiplyPoint3x4(new Vector3(min.x, max.y, 0));
            yield return matrix.MultiplyPoint3x4(new Vector3(max.x, min.y, 0));
            yield return matrix.MultiplyPoint3x4(new Vector3(max.x, max.y, 0));
        }

        public static IEnumerable<Vector3> IterateTransformedVertices(MeshFilter meshFilter, Matrix4x4 matrix)
        {
            var mesh = meshFilter.sharedMesh;

            if (!mesh)
                yield break;

            var vertices = mesh.vertices;

            if (vertices == null)
                yield break;

            if (vertices.Length == 0)
                yield break;

            var tfm = meshFilter.transform;
            matrix *= tfm.localToWorldMatrix;

            for (var i = 0; i < vertices.Length; i++)
                yield return matrix.MultiplyPoint3x4(vertices[i]);
        }

        public static IEnumerable<Vector3> IterateTransformedVertices(SpriteRenderer spriteRenderer, Matrix4x4 matrix)
        {
            var sprite = spriteRenderer.sprite;

            if (!sprite)
                yield break;

            var vertices = sprite.vertices;

            if (vertices == null)
                yield break;

            if (vertices.Length == 0)
                yield break;

            var tfm = spriteRenderer.transform;
            matrix *= tfm.localToWorldMatrix;

            for (var i = 0; i < vertices.Length; i++)
                yield return matrix.MultiplyPoint3x4(vertices[i]);
        }
    }
}
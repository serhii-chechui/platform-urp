using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Utilities.Mesh
{
    public static class MeshSpriteHelper
    {
        private static readonly Vector3[] spriteVertices = new Vector3[4];
        private static readonly Vector3[] spriteNormals = new Vector3[4];
        private static readonly Vector2[] spriteUV = new Vector2[4];
        private static readonly int[] spriteTriangles = {0, 1, 2, 0, 2, 3};

        public static void SetSpriteMesh(UnityEngine.Mesh mesh, Matrix4x4? matrix, Rect position, int positionIndex = 0,
            Rect? uv0 = null, int iuv0Index = 0, Rect? uv1 = null, int iuv1Index = 0, Rect? uv2 = null, int iuv2Index = 0)
        {
            var i = positionIndex % 4 + 4;

            if (matrix.HasValue)
            {
                var matrix4X4 = matrix.Value;
                spriteVertices[(0 + i) % 4] = matrix4X4.MultiplyVector(new Vector3(position.xMin, position.yMin));
                spriteVertices[(1 + i) % 4] = matrix4X4.MultiplyVector(new Vector3(position.xMin, position.yMax));
                spriteVertices[(2 + i) % 4] = matrix4X4.MultiplyVector(new Vector3(position.xMax, position.yMax));
                spriteVertices[(3 + i) % 4] = matrix4X4.MultiplyVector(new Vector3(position.xMax, position.yMin));
            }
            else
            {
                spriteVertices[(0 + i) % 4] = new Vector3(position.xMin, position.yMin);
                spriteVertices[(1 + i) % 4] = new Vector3(position.xMin, position.yMax);
                spriteVertices[(2 + i) % 4] = new Vector3(position.xMax, position.yMax);
                spriteVertices[(3 + i) % 4] = new Vector3(position.xMax, position.yMin);
            }

            mesh.vertices = spriteVertices;

            var normal = Vector3.back;

            if (matrix.HasValue)
                normal = matrix.Value.MultiplyVector(normal);

            spriteNormals[0] = spriteNormals[1] = spriteNormals[2] = spriteNormals[3] = normal;
            mesh.normals = spriteNormals;

            if (uv0.HasValue)
            {
                i = iuv0Index % 4 + 4;
                var rect = uv0.Value;
                spriteUV[(0 + i) % 4] = new Vector2(rect.xMin, rect.yMin);
                spriteUV[(1 + i) % 4] = new Vector2(rect.xMin, rect.yMax);
                spriteUV[(2 + i) % 4] = new Vector2(rect.xMax, rect.yMax);
                spriteUV[(3 + i) % 4] = new Vector2(rect.xMax, rect.yMin);
                mesh.uv = spriteUV;
            }

            // Note: uv1 is obsolete (use uv2 instead)
            if (uv1.HasValue)
            {
                i = iuv1Index % 4 + 4;
                var rect = uv1.Value;
                spriteUV[(0 + i) % 4] = new Vector2(rect.xMin, rect.yMin);
                spriteUV[(1 + i) % 4] = new Vector2(rect.xMin, rect.yMax);
                spriteUV[(2 + i) % 4] = new Vector2(rect.xMax, rect.yMax);
                spriteUV[(3 + i) % 4] = new Vector2(rect.xMax, rect.yMin);
                mesh.uv2 = spriteUV;
            }

            if (uv2.HasValue)
            {
                i = iuv2Index % 4 + 4;
                var rect = uv2.Value;
                spriteUV[(0 + i) % 4] = new Vector2(rect.xMin, rect.yMin);
                spriteUV[(1 + i) % 4] = new Vector2(rect.xMin, rect.yMax);
                spriteUV[(2 + i) % 4] = new Vector2(rect.xMax, rect.yMax);
                spriteUV[(3 + i) % 4] = new Vector2(rect.xMax, rect.yMin);
                mesh.uv2 = spriteUV;
            }

            mesh.triangles = spriteTriangles;

            mesh.RecalculateBounds();
        }

        public static GameObject Sprite(Bounds bounds, bool horizontal = false)
        {
            return Sprite(SpriteRect(bounds, horizontal), horizontal);
        }

        public static GameObject Sprite(Rect rect, bool horizontal = false)
        {
            var obj = new GameObject("sprite", typeof(MeshRenderer));
            var meshFilter = obj.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = SpriteMesh(rect, horizontal);
            return obj;
        }

        public static UnityEngine.Mesh SpriteMesh(Bounds bounds, bool horizontal = false)
        {
            return SpriteMesh(SpriteRect(bounds, horizontal), horizontal);
        }

        public static UnityEngine.Mesh SpriteMesh(Rect rect, bool horizontal = false)
        {
            var mesh = new UnityEngine.Mesh();
            Matrix4x4? matrix = null;

            if (horizontal)
            {
                var m = Matrix4x4.identity;
                m[1, 1] = 0;
                m[2, 1] = 1;
                matrix = m;
            }

            SetSpriteMesh(mesh, matrix, rect, 0, new Rect(0, 0, 1, 1));
            return mesh;
        }

        public static Rect SpriteRect(Bounds bounds, bool horizontal = false)
        {
            if (horizontal)
                return new Rect(bounds.min.x, bounds.min.z, bounds.size.x, bounds.size.z);

            return new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y);
        }

        public static Rect SpriteSubregion(Bounds bounds,
            Rect region, Vector2 size, bool horizontal = false)
        {
            return SpriteSubregion(SpriteRect(bounds, horizontal), region, size);
        }

        public static Rect SpriteSubregion(Rect bounds, Rect region, Vector2 size)
        {
            return new Rect(
                bounds.x + bounds.width * (region.x / size.x),
                bounds.y + bounds.height * (region.y / size.y),
                bounds.width * (region.width / size.x),
                bounds.height * (region.height / size.y));
        }
    }
}
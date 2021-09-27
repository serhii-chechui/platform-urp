using System.Collections;
using System.Collections.Generic;
using Bini.ToolKit.Core.Unity.Extensions;
using UnityEngine;

namespace Bini.ToolKit.Effects.MeshEffects
{
    /// <remarks>
    /// Blend shapes are currently not supported
    /// </remarks>
    public class SparklesEffect : MonoBehaviour
    {
        private static readonly Vector2[] QuadUvs =
        {
            new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0),
        };

        public Material MaterialTemplate;

        public float AnimationSpeed = 1;
        public AnimationCurve IntensityCurve = AnimationCurve.Constant(0, 1, 1);

        public int RandomSeed = 0;
        public int SampleCount = 1000;
        public int SearchLimit = 100;

        public AnimationCurve Distribution = AnimationCurve.Constant(0, 1, 1);

        public bool[] SubmeshMask;

        public float VolumeMaskScale = 1;
        public Vector4[] VolumeMasks;

        private Renderer sourceRenderer;
        private Mesh sourceMesh;

        private Renderer sparklesRenderer;

        private Mesh sparklesMesh;
        private Vector4[] shapeData;
        private float[] phases;

        private void Start()
        {
            InitializeSources();
            CreateSparkles();
        }

        private void Update()
        {
            if (!sparklesMesh) return;

            var time = Time.time * AnimationSpeed;

            IntensityCurve.preWrapMode = WrapMode.Loop;
            IntensityCurve.postWrapMode = WrapMode.Loop;

            var duration = IntensityCurve[IntensityCurve.length - 1].time - IntensityCurve[0].time;

            for (var index = 0; index < phases.Length; index++)
            {
                var intensity = IntensityCurve.Evaluate(phases[index] * duration + time);

                var vertexStart = index * 4;

                for (var cornerIndex = 0; cornerIndex < 4; cornerIndex++)
                    shapeData[vertexStart + cornerIndex].w = intensity;
            }

            sparklesMesh.SetUVs(1, shapeData);
        }

        private void OnDestroy()
        {
            if (sparklesRenderer)
                sparklesRenderer.gameObject.Dispose();
        }

        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying) return;

            Gizmos.color = new Color(1, 0, 0, 0.5f);

            var previousMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;

            foreach (var volumeMask in VolumeMasks)
                Gizmos.DrawSphere(volumeMask * VolumeMaskScale, volumeMask.w * VolumeMaskScale);

            Gizmos.matrix = previousMatrix;
            Gizmos.color = Color.white;
        }

        private void InitializeSources()
        {
            sourceRenderer = GetComponent<Renderer>();
            sourceMesh = gameObject.GetRendererMesh();
        }

        private void CreateSparkles()
        {
            var sourceMeshRenderer = sourceRenderer as MeshRenderer;
            var sourceSkinnedRenderer = sourceRenderer as SkinnedMeshRenderer;

            if (!(sourceMeshRenderer | sourceSkinnedRenderer)) return;

            var sparklesObject = new GameObject("Sparkles");
            sparklesObject.hideFlags = HideFlags.HideAndDontSave;
            sparklesObject.transform.ResetToParent(sourceRenderer.transform);

            var mesh = CreateSparklesMesh();

            if (sourceSkinnedRenderer)
            {
                var skinnedMeshRenderer = sparklesObject.AddComponent<SkinnedMeshRenderer>();

                skinnedMeshRenderer.localBounds = sourceSkinnedRenderer.localBounds;
                skinnedMeshRenderer.quality = sourceSkinnedRenderer.quality;
                skinnedMeshRenderer.rootBone = sourceSkinnedRenderer.rootBone;
                skinnedMeshRenderer.bones = sourceSkinnedRenderer.bones;

                skinnedMeshRenderer.sharedMesh = mesh;

                sparklesRenderer = skinnedMeshRenderer;
            }
            else
            {
                var meshRenderer = sparklesObject.AddComponent<MeshRenderer>();
                var meshFilter = sparklesObject.AddComponent<MeshFilter>();

                meshFilter.mesh = mesh;

                sparklesRenderer = meshRenderer;
            }

            sparklesRenderer.sharedMaterial = MaterialTemplate;

            sparklesMesh = mesh;
        }

        private Mesh CreateSparklesMesh()
        {
            if (!sourceMesh) return null;

            var sourceVertices = sourceMesh.vertices;
            var sourceWeights = sourceMesh.boneWeights;
            var hasWeights = (sourceWeights.Length > 0);

            var random = new System.Random(RandomSeed);

            // In the example, the sparkles were shining in an irregular manner,
            // so it would probably be better to animate them in script

            var volumeMasks = new Vector4[VolumeMasks.Length];

            for (var index = 0; index < volumeMasks.Length; index++)
            {
                volumeMasks[index] = VolumeMasks[index] * VolumeMaskScale;
                volumeMasks[index].w *= volumeMasks[index].w;
            }

            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var boneWeights = hasWeights ? new List<BoneWeight>() : null;
            var uvs = new List<Vector2>();
            var uvs2 = new List<Vector4>();
            var triangles = new List<int>();

            var weightAggregator = new BoneWeightAggregator();

            foreach (var sample in SampleMesh(sourceVertices, random).Sample(SampleCount))
            {
                var vertex0 = sourceVertices[sample.Index0];
                var vertex1 = sourceVertices[sample.Index1];
                var vertex2 = sourceVertices[sample.Index2];

                var barycentric = sample.Barycentric;

                var position = vertex0 * barycentric.x + vertex1 * barycentric.y + vertex2 * barycentric.z;

                if (IsInsideVolumeMask(position, volumeMasks)) continue;

                var normal = sample.Normal;
                var boneWeight = CalculateWeight(sourceWeights, weightAggregator, sample, barycentric);

                var angle = ((float) random.NextDouble()) * Mathf.PI * 2;
                var sin = Mathf.Sin(angle);
                var cos = Mathf.Cos(angle);

                var indexStart = vertices.Count;

                for (var cornerIndex = 0; cornerIndex < 4; cornerIndex++)
                {
                    vertices.Add(position);
                    normals.Add(normal);
                    boneWeights?.Add(boneWeight);

                    uvs.Add(QuadUvs[cornerIndex]);

                    var x = QuadUvs[cornerIndex].x - 0.5f;
                    var y = QuadUvs[cornerIndex].y - 0.5f;
                    uvs2.Add(new Vector4(x * cos - y * sin, x * sin + y * cos));
                }

                triangles.Add(indexStart + 0);
                triangles.Add(indexStart + 1);
                triangles.Add(indexStart + 2);
                triangles.Add(indexStart + 0);
                triangles.Add(indexStart + 2);
                triangles.Add(indexStart + 3);
            }

            var (sizeIndices, sizes) = DistributeSizes(vertices, random, SearchLimit);

            phases = new float[sizes.Length];
            shapeData = uvs2.ToArray();

            for (var index = 0; index < sizes.Length; index++)
            {
                phases[index] = (float) random.NextDouble();

                var vertexStart = sizeIndices[index] * 4;

                for (var cornerIndex = 0; cornerIndex < 4; cornerIndex++)
                    shapeData[vertexStart + cornerIndex].z = sizes[index];
            }

            var mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.boneWeights = boneWeights?.ToArray();
            mesh.SetUVs(0, uvs);
            mesh.SetUVs(1, shapeData);
            mesh.SetTriangles(triangles, 0);
            mesh.bindposes = sourceMesh.bindposes;
            mesh.bounds = sourceMesh.bounds;
            return mesh;
        }

        private bool IsSubmeshIncluded(int submeshIndex)
        {
            if ((SubmeshMask == null) || (SubmeshMask.Length == 0)) return true;
            if (submeshIndex < SubmeshMask.Length) return SubmeshMask[submeshIndex];
            return SubmeshMask[SubmeshMask.Length - 1];
        }

        private MeshSampler SampleMesh(Vector3[] sourceVertices, System.Random random)
        {
            var meshSampler = new MeshSampler(sourceVertices, random);

            for (var submeshIndex = 0; submeshIndex < sourceMesh.subMeshCount; submeshIndex++)
            {
                if (!IsSubmeshIncluded(submeshIndex)) continue;

                meshSampler.AddSubmesh(sourceMesh, submeshIndex);
            }

            return meshSampler;
        }

        private bool IsInsideVolumeMask(Vector3 point, Vector4[] volumeMasks)
        {
            foreach (var volumeMask in volumeMasks)
            {
                var deltaX = point.x - volumeMask.x;
                var deltaY = point.y - volumeMask.y;
                var deltaZ = point.z - volumeMask.z;
                var distance = deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ;

                if (distance <= volumeMask.w) return true;
            }

            return false;
        }

        private BoneWeight CalculateWeight(BoneWeight[] sourceWeights, BoneWeightAggregator weightAggregator,
            in TriangleSample sample, in Vector3 barycentric)
        {
            if ((sourceWeights == null) || (sourceWeights.Length == 0)) return default;

            weightAggregator.Clear();
            weightAggregator.Add(sourceWeights[sample.Index0], barycentric.x);
            weightAggregator.Add(sourceWeights[sample.Index1], barycentric.y);
            weightAggregator.Add(sourceWeights[sample.Index2], barycentric.z);
            return weightAggregator.Get();
        }

        private (int[], float[]) DistributeSizes(List<Vector3> vertices, System.Random random, int searchLimit = 100)
        {
            var positions = new Vector3[vertices.Count / 4];
            var indices = new int[positions.Length];
            var sizes = new float[indices.Length];

            var indexScale = 1f / (indices.Length - 1);

            for (var index = 0; index < indices.Length; index++)
            {
                positions[index] = vertices[index * 4];
                indices[index] = index;
                sizes[index] = Distribution.Evaluate(index * indexScale);
            }

            System.Array.Sort(sizes);

            var processingLimit = Mathf.Max(indices.Length - searchLimit, 0);

            for (var unprocessedCount = indices.Length; unprocessedCount > 0; unprocessedCount--)
            {
                var candidateIndex = (unprocessedCount > processingLimit)
                    ? FindFarthestPoint(indices, positions, unprocessedCount, random, searchLimit)
                    : random.Next(unprocessedCount);

                if (candidateIndex < 0) break;

                var swapIndex = unprocessedCount - 1;

                (indices[candidateIndex], indices[swapIndex]) = (indices[swapIndex], indices[candidateIndex]);
            }

            return (indices, sizes);
        }

        private int FindFarthestPoint(int[] indices, Vector3[] positions, int unprocessedCount,
            System.Random random, int limit)
        {
            if (unprocessedCount <= 0) return -1;
            if (unprocessedCount >= positions.Length) return random.Next(positions.Length);

            var bestIndex = -1;
            var bestDistance = 0f;

            for (var unprocessedIndex = 0; unprocessedIndex < unprocessedCount; unprocessedIndex++)
            {
                var index = indices[unprocessedIndex];
                var minDistance = FindMinDistance(indices, positions, unprocessedCount, positions[index], limit);

                if (minDistance > bestDistance)
                {
                    bestDistance = minDistance;
                    bestIndex = unprocessedIndex;
                }
            }

            return bestIndex;
        }

        private static float FindMinDistance(int[] indices, Vector3[] positions, int unprocessedCount,
            Vector3 point, int limit)
        {
            var minDistance = float.PositiveInfinity;

            if (limit > 0)
                unprocessedCount = Mathf.Max(unprocessedCount, positions.Length - limit);

            for (var processedIndex = unprocessedCount; processedIndex < positions.Length; processedIndex++)
            {
                var index = indices[processedIndex];
                var deltaX = point.x - positions[index].x;
                var deltaY = point.y - positions[index].y;
                var deltaZ = point.z - positions[index].z;
                var distance = deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ;

                if (distance < minDistance)
                    minDistance = distance;
            }

            return minDistance;
        }

        private class MeshSampler : IEnumerable<TriangleInfo>
        {
            // 32-bit float has 23 bits of precision anyway
            private const int RandomMax = (1 << 23);
            private const float RandomScale = 1f / RandomMax;

            private readonly Vector3[] vertices;

            private List<int> submeshIndices = new List<int>();

            private List<TriangleInfo> triangleInfos = new List<TriangleInfo>();

            private float totalArea;

            private System.Random random;

            public float Area => totalArea;

            public int TriangleCount => triangleInfos.Count;

            public TriangleInfo this[int index] => triangleInfos[index];

            public MeshSampler(Vector3[] vertices, System.Random random)
            {
                this.vertices = vertices;
                this.random = random;
            }

            public MeshSampler(Vector3[] vertices, int seed = 0) : this(vertices, new System.Random(seed))
            {
            }

            public void AddTriangle(int index0, int index1, int index2)
            {
                var vertex0 = vertices[index0];
                var vertex1 = vertices[index1];
                var vertex2 = vertices[index2];

                var delta1 = vertex1 - vertex0;
                var delta2 = vertex2 - vertex0;
                var direction = Vector3.Cross(delta1, delta2);
                var area = direction.magnitude;

                if (area <= float.Epsilon) return;

                var triangleInfo = new TriangleInfo
                {
                    Index0 = index0,
                    Index1 = index1,
                    Index2 = index2,
                    Direction = direction,
                    Area = area,
                };
                triangleInfos.Add(triangleInfo);

                totalArea += area;
            }

            public void AddSubmesh(Mesh mesh, int submeshIndex)
            {
                var descriptor = mesh.GetSubMesh(submeshIndex);

                if (descriptor.topology == MeshTopology.Triangles)
                {
                    mesh.GetIndices(submeshIndices, submeshIndex, true);

                    for (var primitiveStart = 0; primitiveStart < submeshIndices.Count; primitiveStart += 3)
                    {
                        var index0 = submeshIndices[primitiveStart + 0];
                        var index1 = submeshIndices[primitiveStart + 1];
                        var index2 = submeshIndices[primitiveStart + 2];
                        AddTriangle(index0, index1, index2);
                    }
                }
                else if (descriptor.topology == MeshTopology.Quads)
                {
                    mesh.GetIndices(submeshIndices, submeshIndex, true);

                    for (var primitiveStart = 0; primitiveStart < submeshIndices.Count; primitiveStart += 4)
                    {
                        var index0 = submeshIndices[primitiveStart + 0];
                        var index1 = submeshIndices[primitiveStart + 1];
                        var index2 = submeshIndices[primitiveStart + 2];
                        var index3 = submeshIndices[primitiveStart + 3];
                        AddTriangle(index0, index1, index2);
                        AddTriangle(index0, index2, index3);
                    }
                }
            }

            public IEnumerable<TriangleSample> Sample(int count)
            {
                if (triangleInfos.Count == 0) yield break;

                var areaScale = count / totalArea;
                var currentCount = 0;
                var currentArea = 0f;

                foreach (var triangleInfo in triangleInfos)
                {
                    var newArea = currentArea + triangleInfo.Area;
                    var newCount = (int) (newArea * areaScale);
                    var deltaCount = newCount - currentCount;
                    currentArea = newArea;
                    currentCount = newCount;

                    if (deltaCount <= 0) continue;

                    var sample = new TriangleSample
                    {
                        Index0 = triangleInfo.Index0,
                        Index1 = triangleInfo.Index1,
                        Index2 = triangleInfo.Index2,
                        Normal = triangleInfo.Normal,
                    };

                    for (; deltaCount > 0; deltaCount--)
                    {
                        sample.Barycentric = GetRandomBarycentric();
                        yield return sample;
                    }
                }
            }

            public IEnumerator<TriangleInfo> GetEnumerator() => triangleInfos.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public float GetRandomFloat()
            {
                return random.Next(RandomMax) * RandomScale;
            }

            public Vector3 GetRandomBarycentric()
            {
                var x = random.Next(RandomMax) * RandomScale;
                var y = random.Next(RandomMax) * RandomScale;

                if (x + y >= 1f)
                {
                    x = 1f - x;
                    y = 1f - y;
                }

                return new Vector3(x, y, 1f - x - y);
            }
        }

        private struct TriangleInfo
        {
            public int Index0;
            public int Index1;
            public int Index2;
            public Vector3 Direction;
            public float Area;
            public Vector3 Normal => Area > float.Epsilon ? Direction / Area : default;
        }

        private struct TriangleSample
        {
            public int Index0;
            public int Index1;
            public int Index2;
            public Vector3 Barycentric;
            public Vector3 Normal;
        }
    }
}
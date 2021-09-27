using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Utilities.Mesh
{
    public static class SkinnedMeshHelper
    {
        [ThreadStatic] private static Matrix4x4[] boneMatrices = new Matrix4x4[64];

#if UNITY_2019 || UNITY_2019_1_OR_NEWER
        public static int GetMaxBoneCount(SkinnedMeshRenderer skin = null)
        {
            if (!skin) return GetMaxBoneCount(QualitySettings.skinWeights);
            return GetMaxBoneCount(skin.quality);
        }
        public static int GetMaxBoneCount(SkinQuality skinQuality)
        {
            if (skinQuality == SkinQuality.Bone1) return 1;
            if (skinQuality == SkinQuality.Bone2) return 2;
            if (skinQuality == SkinQuality.Bone4) return 4;
            return GetMaxBoneCount(QualitySettings.skinWeights); // SkinQuality.Auto
        }
        public static int GetMaxBoneCount(SkinWeights skinWeights)
        {
            if (skinWeights == SkinWeights.OneBone) return 1;
            if (skinWeights == SkinWeights.TwoBones) return 2;
            if (skinWeights == SkinWeights.FourBones) return 4;
            return 0; // should never happen
        }
#else
        public static int GetMaxBoneCount(SkinnedMeshRenderer skin = null)
        {
            if (!skin)
                return GetMaxBoneCount(QualitySettings.blendWeights);

            return GetMaxBoneCount(skin.quality);
        }

        public static int GetMaxBoneCount(SkinQuality skinQuality)
        {
            switch (skinQuality)
            {
                case SkinQuality.Bone1:
                    return 1;

                case SkinQuality.Bone2:
                    return 2;

                case SkinQuality.Bone4:
                    return 4;

                default:
                    return GetMaxBoneCount(QualitySettings.blendWeights);
            }
        }

        public static int GetMaxBoneCount(BlendWeights boneWeights)
        {
            switch (boneWeights)
            {
                case BlendWeights.OneBone:
                    return 1;

                case BlendWeights.TwoBones:
                    return 2;

                case BlendWeights.FourBones:
                    return 4;

                default:
                    return 0; // should never happen
            }
        }
#endif

        public static Vector3[] GetSkinnedVertices(SkinnedMeshRenderer skin, Matrix4x4 matrix, int maxBones = -1)
        {
            if (!skin)
                return null;

            var mesh = skin.sharedMesh;

            if (!mesh)
                return null;

            var vertices = mesh.vertices;

            if (vertices == null)
                return null;

            var newVert = new Vector3[vertices.Length];
            var i = 0;

            foreach (var v in IterateSkinnedVertices(skin, matrix, maxBones))
            {
                newVert[i] = v;
                ++i;
            }

            return newVert;
        }

        internal static IEnumerable<Vector3> IterateSkinnedVertices(SkinnedMeshRenderer skin, Matrix4x4 matrix,
            int maxBones = -1)
        {
            var mesh = skin.sharedMesh;

            if (!mesh)
                yield break;

            var vertices = mesh.vertices;

            if (vertices == null)
                yield break;

            if (vertices.Length == 0)
                yield break;

            var bindposes = mesh.bindposes;

            if (bindposes == null)
                yield break;

            var boneWeights = mesh.boneWeights;

            if (boneWeights == null)
                yield break;

            var bones = skin.bones;

            if (bones == null)
                yield break;

            var tfm = skin.transform;

            if (bones.Length > boneMatrices.Length)
                boneMatrices = new Matrix4x4[bones.Length];

            for (var i = 0; i < bones.Length; i++)
                boneMatrices[i] =
                    matrix * ((bones[i] ? bones[i].localToWorldMatrix : tfm.localToWorldMatrix) * bindposes[i]);

            if (maxBones < 0)
                maxBones = GetMaxBoneCount(skin);

            for (var i = 0; i < vertices.Length; i++)
            {
                var bw = boneWeights[i];
                var srcPos = vertices[i];
                var dstPos = Vector3.zero;
                var w = 0f;

                if ((bw.weight0 > 0) & (maxBones > 0))
                {
                    dstPos += boneMatrices[bw.boneIndex0].MultiplyPoint3x4(srcPos) * bw.weight0;
                    w += bw.weight0;
                }

                if ((bw.weight1 > 0) & (maxBones > 1))
                {
                    dstPos += boneMatrices[bw.boneIndex1].MultiplyPoint3x4(srcPos) * bw.weight1;
                    w += bw.weight1;
                }

                if ((bw.weight2 > 0) & (maxBones > 2))
                {
                    dstPos += boneMatrices[bw.boneIndex2].MultiplyPoint3x4(srcPos) * bw.weight2;
                    w += bw.weight2;
                }

                if ((bw.weight3 > 0) & (maxBones > 3))
                {
                    dstPos += boneMatrices[bw.boneIndex3].MultiplyPoint3x4(srcPos) * bw.weight3;
                    w += bw.weight3;
                }

                yield return w > 0f ? dstPos / w : matrix.MultiplyPoint3x4(srcPos);
            }
        }
    }
}
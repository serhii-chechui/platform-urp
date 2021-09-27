using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Extensions
{
    public static class BoneWeightExtensions
    {
        public static void Get(this BoneWeight boneWeight, int index, out int boneIndex, out float weight)
        {
            switch (index)
            {
                case 0:
                    boneIndex = boneWeight.boneIndex0;
                    weight = boneWeight.weight0;
                    return;
                case 1:
                    boneIndex = boneWeight.boneIndex1;
                    weight = boneWeight.weight1;
                    return;
                case 2:
                    boneIndex = boneWeight.boneIndex2;
                    weight = boneWeight.weight2;
                    return;
                case 3:
                    boneIndex = boneWeight.boneIndex3;
                    weight = boneWeight.weight3;
                    return;
                default:
                    boneIndex = -1;
                    weight = 0f;
                    return;
            }
        }

        public static int GetIndex(this BoneWeight boneWeight, int index)
        {
            switch (index)
            {
                case 0:
                    return boneWeight.boneIndex0;
                case 1:
                    return boneWeight.boneIndex1;
                case 2:
                    return boneWeight.boneIndex2;
                case 3:
                    return boneWeight.boneIndex3;
            }

            return -1;
        }

        public static float GetWeight(this BoneWeight boneWeight, int index)
        {
            switch (index)
            {
                case 0:
                    return boneWeight.weight0;
                case 1:
                    return boneWeight.weight1;
                case 2:
                    return boneWeight.weight2;
                case 3:
                    return boneWeight.weight3;
            }

            return 0f;
        }

        public static BoneWeight Normalized(this BoneWeight boneWeight)
        {
            var sum = boneWeight.SumWeight();

            if (sum == 0)
                return boneWeight;

            boneWeight.weight0 /= sum;
            boneWeight.weight1 /= sum;
            boneWeight.weight2 /= sum;
            boneWeight.weight3 /= sum;
            return boneWeight;
        }

        public static BoneWeight Set(this BoneWeight boneWeight, int index, int boneIndex, float weight)
        {
            switch (index)
            {
                case 0:
                    boneWeight.boneIndex0 = boneIndex;
                    boneWeight.weight0 = weight;
                    break;
                case 1:
                    boneWeight.boneIndex1 = boneIndex;
                    boneWeight.weight1 = weight;
                    break;
                case 2:
                    boneWeight.boneIndex2 = boneIndex;
                    boneWeight.weight2 = weight;
                    break;
                case 3:
                    boneWeight.boneIndex3 = boneIndex;
                    boneWeight.weight3 = weight;
                    break;
            }

            return boneWeight;
        }

        public static BoneWeight SetIndex(this BoneWeight boneWeight, int index, int value)
        {
            switch (index)
            {
                case 0:
                    boneWeight.boneIndex0 = value;
                    break;
                case 1:
                    boneWeight.boneIndex1 = value;
                    break;
                case 2:
                    boneWeight.boneIndex2 = value;
                    break;
                case 3:
                    boneWeight.boneIndex3 = value;
                    break;
            }

            return boneWeight;
        }

        public static BoneWeight SetWeight(this BoneWeight boneWeight, int index, float value)
        {
            switch (index)
            {
                case 0:
                    boneWeight.weight0 = value;
                    break;
                case 1:
                    boneWeight.weight1 = value;
                    break;
                case 2:
                    boneWeight.weight2 = value;
                    break;
                case 3:
                    boneWeight.weight3 = value;
                    break;
            }

            return boneWeight;
        }

        public static float SumWeight(this BoneWeight boneWeight)
        {
            return boneWeight.weight0 + boneWeight.weight1 + boneWeight.weight2 + boneWeight.weight3;
        }
    }
}
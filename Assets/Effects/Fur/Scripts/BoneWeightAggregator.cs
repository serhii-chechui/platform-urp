using System.Collections.Generic;
using Bini.ToolKit.Core.Unity.Extensions;
using UnityEngine;

namespace Bini.ToolKit.Effects.MeshEffects
{
    internal class BoneWeightAggregator
    {
        private readonly List<(int id, float weight)> data = new List<(int, float)>(12);

        public void Clear()
        {
            data.Clear();
        }
            
        public void Add(in BoneWeight boneWeight, float factor)
        {
            for (var weightIndex = 0; weightIndex < 4; ++weightIndex)
            {
                boneWeight.Get(weightIndex, out var id, out var weight);
                
                if ((id >= 0) & (weight > 0))
                    Add(id, weight * factor);
            }
        }

        public void Add(int id, float weight)
        {
            for (var index = 0; index < data.Count; ++index)
            {
                if (data[index].id != id)
                    continue;
                
                data[index] = (id, data[index].weight + weight);
                
                return;
            }

            data.Add((id, weight));
        }

        public BoneWeight Get()
        {
            data.Sort((itemA, itemB) => itemB.weight.CompareTo(itemA.weight));
            var boneWeight = default(BoneWeight);
            Normalize(ref boneWeight);
            return boneWeight;
        }
            
        private bool Normalize(ref BoneWeight boneWeight)
        {
            var count = Mathf.Min(data.Count, 4);
            var norm = 0f;

            for (var index = 0; index < count; ++index)
            {
                if (data[index].weight <= 0)
                    data[index] = (data[index].id, 0);
                else
                    norm += data[index].weight;
            }

            if (norm <= 1e-6f) return false;

            norm = 1f / norm;

            for (var index = 0; index < count; ++index)
                boneWeight = boneWeight.Set(index, data[index].id, data[index].weight * norm);

            return true;
        }
    }
}
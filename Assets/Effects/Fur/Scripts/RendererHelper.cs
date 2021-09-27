using System.Collections.Generic;
using UnityEngine;

namespace Bini.ToolKit.Effects.MeshEffects
{
    internal static class RendererHelper
    {
        public static void CloneMain(Renderer source, Renderer target, bool propertyBlocks, bool instanceData)
        {
            if (!(source && target)) return;

            Clone(source, target, propertyBlocks);

            switch (source)
            {
                case MeshRenderer meshRenderer:
                    Clone(meshRenderer, target as MeshRenderer);
                    break;
                case SkinnedMeshRenderer skinnedMeshRenderer:
                    Clone(skinnedMeshRenderer, target as SkinnedMeshRenderer, instanceData);
                    break;
                case SpriteRenderer spriteRenderer:
                    Clone(spriteRenderer, target as SpriteRenderer);
                    break;
                case BillboardRenderer billboardRenderer:
                    Clone(billboardRenderer, target as BillboardRenderer);
                    break;
                case LineRenderer lineRenderer:
                    Clone(lineRenderer, target as LineRenderer, instanceData);
                    break;
                case TrailRenderer trailRenderer:
                    Clone(trailRenderer, target as TrailRenderer, instanceData);
                    break;
                case ParticleSystemRenderer particleSystemRenderer:
                    Clone(particleSystemRenderer, target as ParticleSystemRenderer);
                    break;
                default:
                    throw new System.ArgumentException($"Unrecognized renderer type {source.GetType()}");
            }
        }

        public static void Clone(Renderer source, Renderer target, bool propertyBlocks)
        {
            if (!(source && target)) return;

            target.enabled = source.enabled;

            var materials = source.sharedMaterials;
            target.sharedMaterials = materials;

            target.rendererPriority = source.rendererPriority;
            target.renderingLayerMask = source.renderingLayerMask;

            target.sortingLayerID = source.sortingLayerID;
            target.sortingLayerName = source.sortingLayerName;
            target.sortingOrder = source.sortingOrder;

            target.allowOcclusionWhenDynamic = source.allowOcclusionWhenDynamic;

            target.lightmapIndex = source.lightmapIndex;
            target.lightmapScaleOffset = source.lightmapScaleOffset;
            target.lightProbeProxyVolumeOverride = source.lightProbeProxyVolumeOverride;
            target.lightProbeUsage = source.lightProbeUsage;
            target.motionVectorGenerationMode = source.motionVectorGenerationMode;
            target.probeAnchor = source.probeAnchor;
            target.realtimeLightmapIndex = source.realtimeLightmapIndex;
            target.realtimeLightmapScaleOffset = source.realtimeLightmapScaleOffset;
            target.receiveShadows = source.receiveShadows;
            target.reflectionProbeUsage = source.reflectionProbeUsage;
            target.shadowCastingMode = source.shadowCastingMode;

#if UNITY_2019_3_OR_NEWER
            target.forceRenderingOff = source.forceRenderingOff;
            target.rayTracingMode = source.rayTracingMode;
#endif

            if (!(propertyBlocks && source.HasPropertyBlock())) return;

            var propertyBlock = new MaterialPropertyBlock();

            source.GetPropertyBlock(propertyBlock);
            target.SetPropertyBlock(propertyBlock.isEmpty ? null : propertyBlock);

            for (var index = 0; index < materials.Length; index++)
            {
                source.GetPropertyBlock(propertyBlock, index);
                target.SetPropertyBlock(propertyBlock.isEmpty ? null : propertyBlock, index);
            }
        }

        public static void Clone(MeshRenderer source, MeshRenderer target)
        {
            if (!(source && target)) return;

            var sourceMeshFilter = source.GetComponent<MeshFilter>();

            if (sourceMeshFilter)
            {
                var targetMeshFilter = target.GetComponent<MeshFilter>();

                if (!targetMeshFilter)
                    targetMeshFilter = target.gameObject.AddComponent<MeshFilter>();

                targetMeshFilter.sharedMesh = sourceMeshFilter.sharedMesh;
            }

            target.additionalVertexStreams = source.additionalVertexStreams;

#if UNITY_2019_2_OR_NEWER && UNITY_EDITOR
            target.receiveGI = source.receiveGI;
#endif

#if UNITY_2019_3_OR_NEWER && UNITY_EDITOR
            target.scaleInLightmap = source.scaleInLightmap;
            target.stitchLightmapSeams = source.stitchLightmapSeams;
#endif

#if UNITY_2020_2_OR_NEWER
            target.enlightenVertexStream = source.enlightenVertexStream;
#endif
        }

        public static void Clone(SkinnedMeshRenderer source, SkinnedMeshRenderer target, bool instanceData)
        {
            if (!(source && target)) return;

            var mesh = source.sharedMesh;
            target.sharedMesh = mesh;
            target.localBounds = source.localBounds;
            target.quality = source.quality;
            target.rootBone = source.rootBone;
            target.bones = source.bones;
            target.skinnedMotionVectors = source.skinnedMotionVectors;
            target.updateWhenOffscreen = source.updateWhenOffscreen;
            target.forceMatrixRecalculationPerRender = source.forceMatrixRecalculationPerRender;

            if (!(instanceData && mesh)) return;

            for (var index = 0; index < mesh.blendShapeCount; index++)
                target.SetBlendShapeWeight(index, source.GetBlendShapeWeight(index));
        }

        public static void Clone(SpriteRenderer source, SpriteRenderer target)
        {
            if (!(source && target)) return;

            target.sprite = source.sprite;
            target.color = source.color;
            target.flipX = source.flipX;
            target.flipY = source.flipY;
            target.maskInteraction = source.maskInteraction;
            target.spriteSortPoint = source.spriteSortPoint;
            target.drawMode = source.drawMode;
            target.size = source.size;
            target.tileMode = source.tileMode;
            target.adaptiveModeThreshold = source.adaptiveModeThreshold;
        }

        public static void Clone(BillboardRenderer source, BillboardRenderer target)
        {
            if (!(source && target)) return;

            target.billboard = source.billboard;
        }

        public static void Clone(LineRenderer source, LineRenderer target, bool instanceData)
        {
            if (!(source && target)) return;

            target.alignment = source.alignment;
            target.colorGradient = source.colorGradient;
            target.endColor = source.endColor;
            target.endWidth = source.endWidth;
            target.generateLightingData = source.generateLightingData;
            target.loop = source.loop;
            target.numCapVertices = source.numCapVertices;
            target.numCornerVertices = source.numCornerVertices;
            target.positionCount = source.positionCount;
            target.shadowBias = source.shadowBias;
            target.startColor = source.startColor;
            target.startWidth = source.startWidth;
            target.textureMode = source.textureMode;
            target.useWorldSpace = source.useWorldSpace;
            target.widthCurve = source.widthCurve;
            target.widthMultiplier = source.widthMultiplier;

            if (!instanceData) return;

            var positions = new Vector3[source.positionCount];
            source.GetPositions(positions);
            target.SetPositions(positions);
        }

        public static void Clone(TrailRenderer source, TrailRenderer target, bool instanceData)
        {
            if (!(source && target)) return;

            target.alignment = source.alignment;
            target.autodestruct = source.autodestruct;
            target.colorGradient = source.colorGradient;
            target.emitting = source.emitting;
            target.endColor = source.endColor;
            target.endWidth = source.endWidth;
            target.generateLightingData = source.generateLightingData;
            target.minVertexDistance = source.minVertexDistance;
            target.numCapVertices = source.numCapVertices;
            target.numCornerVertices = source.numCornerVertices;
            target.shadowBias = source.shadowBias;
            target.startColor = source.startColor;
            target.startWidth = source.startWidth;
            target.textureMode = source.textureMode;
            target.time = source.time;
            target.widthCurve = source.widthCurve;
            target.widthMultiplier = source.widthMultiplier;

            if (!instanceData) return;

            // Not sure if this makes sense (timestamps can't be copied anyway)
            var positions = new Vector3[source.positionCount];
            source.GetPositions(positions);
            target.Clear();
            target.AddPositions(positions);
        }

        public static void Clone(ParticleSystemRenderer source, ParticleSystemRenderer target)
        {
            if (!(source && target)) return;

            target.alignment = source.alignment;
            target.allowRoll = source.allowRoll;
            target.cameraVelocityScale = source.cameraVelocityScale;
            target.enableGPUInstancing = source.enableGPUInstancing;
            target.flip = source.flip;
            target.lengthScale = source.lengthScale;
            target.maskInteraction = source.maskInteraction;
            target.maxParticleSize = source.maxParticleSize;
            target.minParticleSize = source.minParticleSize;
            target.normalDirection = source.normalDirection;
            target.pivot = source.pivot;
            target.renderMode = source.renderMode;
            target.shadowBias = source.shadowBias;
            target.sortingFudge = source.sortingFudge;
            target.sortMode = source.sortMode;
            target.trailMaterial = source.trailMaterial;
            target.velocityScale = source.velocityScale;

            var streams = new List<ParticleSystemVertexStream>();
            source.GetActiveVertexStreams(streams);
            target.SetActiveVertexStreams(streams);

            if (source.meshCount <= 1)
            {
                target.mesh = source.mesh;
            }
            else if (source.meshCount > 1)
            {
                var meshes = new Mesh[source.meshCount];
                source.GetMeshes(meshes);
                target.SetMeshes(meshes);
            }
        }
    }
}
using System.Collections.Generic;
using Bini.ToolKit.Core.Unity.Extensions;
using UnityEngine;

namespace Bini.ToolKit.Effects.MeshEffects
{
    /// <remarks>
    /// Currently, the effect is implemented by making a separate object for each layer.
    /// Technically, having a single mesh with all the layers would save us some
    /// computations, but the code would be a lot more complicated.
    /// </remarks>
    /// <remarks>
    /// Blend shapes are currently not supported
    /// </remarks>
    [ExecuteAlways]
    public class FurEffect : MonoBehaviour
    {
        private static class ShaderId
        {
            public static readonly int MainTex = Shader.PropertyToID("_MainTex");
            public static readonly int MainTexSt = Shader.PropertyToID("_MainTex_ST");
            public static readonly int Slice = Shader.PropertyToID("_Slice");
            public static readonly int LayerCount = Shader.PropertyToID("_LayerCount");
        }

        public Material MaterialTemplate;

        private Material currentMaterialTemplate;

        private Renderer sourceRenderer;
        private Mesh sourceMesh;

        private List<Renderer> cloneRenderers = new List<Renderer>();

        private MaterialPropertyBlock[] propertyBlocks;

        private void Start()
        {
            InitializeSources();
            UpdateLayers();
        }

        private void OnEnable()
        {
            if (Application.isPlaying) return;

            InitializeSources();
            UpdateLayers();
        }

        private void LateUpdate()
        {
            UpdateLayers();
        }

        private void OnDisable()
        {
            if (Application.isPlaying)
                UpdateLayers(false);
            else
                ClearLayers();
        }

        private void OnDestroy()
        {
            ClearLayers();
        }

        private void InitializeSources()
        {
            sourceRenderer = GetComponent<Renderer>();
            sourceMesh = gameObject.GetRendererMesh();
        }

        private void ClearLayers()
        {
            foreach (var cloneRenderer in cloneRenderers)
            {
                if (!cloneRenderer) continue;

                cloneRenderer.gameObject.Dispose();
            }

            cloneRenderers.Clear();
        }

        private void UpdateLayers(bool isEnabled = true)
        {
            if (!(sourceRenderer && MaterialTemplate))
            {
                ClearLayers();
                return;
            }

            if (currentMaterialTemplate != MaterialTemplate)
            {
                currentMaterialTemplate = MaterialTemplate;
                ClearLayers();
            }

            isEnabled &= sourceRenderer.enabled;

            var layerCount = MaterialTemplate.GetInt(ShaderId.LayerCount);

            for (var index = 0; index < layerCount; index++)
            {
                if (index >= cloneRenderers.Count)
                    cloneRenderers.Add(CreateLayer());

                var cloneRenderer = cloneRenderers[index];
                cloneRenderer.enabled = isEnabled;
                UpdateMaterials(cloneRenderer, (index + 0.5f) / layerCount);
            }

            for (var index = layerCount; index < cloneRenderers.Count; index++)
                cloneRenderers[index].enabled = false;
        }

        private Renderer CreateLayer()
        {
            if (cloneRenderers.Count == 0)
            {
                var cloneRenderer = CloneRenderer(sourceRenderer, sourceMesh);
                SetupMaterials(cloneRenderer);
                return cloneRenderer;
            }
            else
            {
                var cloneRenderer = Instantiate(cloneRenderers[0]);
                cloneRenderer.gameObject.hideFlags = HideFlags.HideAndDontSave;
                cloneRenderer.transform.ResetToParent(transform);
                return cloneRenderer;
            }
        }

        private void UpdateMaterials(Renderer cloneRenderer, float slicePosition)
        {
            for (var index = 0; index < sourceMesh.subMeshCount; index++)
            {
                var propertyBlock = propertyBlocks[index];

                propertyBlock.SetFloat(ShaderId.Slice, slicePosition);

                cloneRenderer.SetPropertyBlock(propertyBlock, index);
            }
        }

        private void SetupMaterials(Renderer cloneRenderer)
        {
            var materials = sourceRenderer.sharedMaterials;

            if ((propertyBlocks == null) || (propertyBlocks.Length < materials.Length))
                System.Array.Resize(ref propertyBlocks, materials.Length);

            cloneRenderer.sharedMaterial = MaterialTemplate;

            for (var index = 0; index < sourceMesh.subMeshCount; index++)
            {
                if (propertyBlocks[index] == null)
                    propertyBlocks[index] = new MaterialPropertyBlock();

                var propertyBlock = propertyBlocks[index];

                var materialIndex = Mathf.Min(index, materials.Length - 1);

                sourceRenderer.GetPropertyBlock(propertyBlock, index);

                var mainTexture = propertyBlock.GetTexture(ShaderId.MainTex);

                if (!mainTexture && (materialIndex >= 0))
                    propertyBlock.SetTexture(ShaderId.MainTex, materials[materialIndex].mainTexture);

                var mainTextureSt = propertyBlock.GetVector(ShaderId.MainTexSt);

                if ((mainTextureSt == default) && (materialIndex >= 0))
                {
                    var scale = materials[materialIndex].mainTextureScale;
                    var offset = materials[materialIndex].mainTextureOffset;
                    mainTextureSt = new Vector4(scale.x, scale.y, offset.x, offset.y);
                    propertyBlock.SetVector(ShaderId.MainTexSt, mainTextureSt);
                }

                cloneRenderer.SetPropertyBlock(propertyBlock, index);
            }
        }

        private static Renderer CloneRenderer(Renderer sourceRenderer, Mesh sourceMesh)
        {
            var isMeshRenderer = sourceRenderer is MeshRenderer;
            var isSkinnedRenderer = sourceRenderer is SkinnedMeshRenderer;

            if (!(isMeshRenderer | isSkinnedRenderer)) return null;

            var cloneObject = new GameObject("FurOverlay");
            cloneObject.hideFlags = HideFlags.HideAndDontSave;
            cloneObject.transform.ResetToParent(sourceRenderer.transform);

            var rendererType = isMeshRenderer ? typeof(MeshRenderer) : typeof(SkinnedMeshRenderer);
            var cloneRenderer = cloneObject.AddComponent(rendererType) as Renderer;
            RendererHelper.CloneMain(sourceRenderer, cloneRenderer, true, true);

            if (isSkinnedRenderer)
                (cloneRenderer as SkinnedMeshRenderer).sharedMesh = sourceMesh;
            else
                cloneRenderer.EnsureComponent<MeshFilter>().sharedMesh = sourceMesh;

            return cloneRenderer;
        }
    }
}
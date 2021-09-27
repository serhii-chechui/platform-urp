using System.Collections.Generic;
using Bini.ToolKit.Core.Unity.Utilities.Mesh;
using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Extensions
{
    public static class ComponentExtensions
    {
        public static T EnsureComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.EnsureComponent<T>();
        }

        public static Component EnsureComponent(this Component component, System.Type type)
        {
            return component.gameObject.EnsureComponent(type);
        }

        public static IEnumerable<T> EnumerateComponents<T>(this Component component, bool multicomponent = true)
            where T : Component
        {
            return component.gameObject.EnumerateComponents<T>(multicomponent);
        }

        public static IEnumerable<Component> EnumerateComponents(this Component component, System.Type type,
            bool multicomponent = true)
        {
            return component.gameObject.EnumerateComponents(type, multicomponent);
        }

        /// <summary>
        /// Enumerates over the components of specified type in the given enumerable.
        /// Note: there is no check for uniqueness, so the same component might be returned multiple times.
        /// </summary>
        public static IEnumerable<T> ToComponents<T>(this IEnumerable<Component> components, bool multicomponent = true)
            where T : Component
        {
            foreach (var component in components)
            {
                if (!component.IsAlive()) continue;

                foreach (var result in component.gameObject.EnumerateComponents<T>(multicomponent))
                {
                    if (result.IsAlive()) yield return result;
                }
            }
        }

        /// <summary>
        /// Enumerates over the GameObjects in the given enumerable.
        /// Note: there is no check for uniqueness, so the same object might be returned multiple times.
        /// </summary>
        public static IEnumerable<GameObject> ToGameObjects(this IEnumerable<Component> components)
        {
            foreach (var component in components)
            {
                if (component.IsAlive()) yield return component.gameObject;
            }
        }

        public static bool IsEnabled(this Component component, bool checkGameObject = false)
        {
            if (!component.IsAlive()) return false;

            if (checkGameObject && !component.gameObject.activeInHierarchy) return false;

            switch (component)
            {
                case Behaviour behaviour:
                    return behaviour.enabled;
                case Collider collider:
                    return collider.enabled;
                case Renderer renderer:
                    return renderer.enabled;
                case ParticleSystem particleSystem:
                    return particleSystem.emission.enabled;
                case LODGroup lodGroup:
                    return lodGroup.enabled;
                default:
                    return true;
            }
        }

        public static void SetEnabled(this Component component, bool enabled)
        {
            if (!component.IsAlive()) return;

            switch (component)
            {
                case Behaviour behaviour:
                    behaviour.enabled = enabled;
                    break;
                case Collider collider:
                    collider.enabled = enabled;
                    break;
                case Renderer renderer:
                    renderer.enabled = enabled;
                    break;
                case ParticleSystem particleSystem:
                    var emission = particleSystem.emission;
                    emission.enabled = enabled;
                    break;
                case LODGroup lodGroup:
                    lodGroup.enabled = enabled;
                    break;
            }
        }

        public static void CopyTo(this Renderer source, GameObject gameObject)
        {
            if (!source.IsAlive()) return;

            if (source is SkinnedMeshRenderer sourceSkinnedRenderer)
            {
                var destinationSkinnedRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
                destinationSkinnedRenderer.rootBone = sourceSkinnedRenderer.rootBone;
                destinationSkinnedRenderer.bones = sourceSkinnedRenderer.bones;
                destinationSkinnedRenderer.quality = sourceSkinnedRenderer.quality;
                destinationSkinnedRenderer.updateWhenOffscreen = sourceSkinnedRenderer.updateWhenOffscreen;
                destinationSkinnedRenderer.sharedMesh = sourceSkinnedRenderer.sharedMesh;
            }
            else
            {
                var sourceMeshFilter = source.GetComponent<MeshFilter>();
                var destinationMeshFilter = gameObject.AddComponent<MeshFilter>();
                destinationMeshFilter.sharedMesh = sourceMeshFilter.sharedMesh;
                gameObject.AddComponent<MeshRenderer>(); // no settings to initialize
            }

            var destination = gameObject.GetComponent<Renderer>();
            destination.shadowCastingMode = source.shadowCastingMode;
            destination.receiveShadows = source.receiveShadows;
            destination.lightProbeUsage = source.lightProbeUsage;
            destination.sharedMaterial = source.sharedMaterial;
        }

        public static Vector3[] GetSkinnedVertices(this SkinnedMeshRenderer skinnedMeshRenderer, Space space,
            int maxBones = -1)
        {
            var matrix = (space == Space.Self) ? skinnedMeshRenderer.transform.worldToLocalMatrix : Matrix4x4.identity;
            return SkinnedMeshHelper.GetSkinnedVertices(skinnedMeshRenderer, matrix, maxBones);
        }

        public static Vector3[] GetSkinnedVertices(this SkinnedMeshRenderer skinnedMeshRenderer, Matrix4x4 matrix,
            int maxBones = -1)
        {
            return SkinnedMeshHelper.GetSkinnedVertices(skinnedMeshRenderer, matrix, maxBones);
        }
    }
}
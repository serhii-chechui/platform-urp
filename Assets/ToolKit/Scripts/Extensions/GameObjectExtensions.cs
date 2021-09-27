using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Extensions
{
    public static class GameObjectExtensions
    {
        public static T EnsureComponent<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();

            if (!component.IsAlive())
                component = gameObject.AddComponent<T>();

            return component;
        }

        public static Component EnsureComponent(this GameObject gameObject, System.Type type)
        {
            var component = gameObject.GetComponent(type);

            if (!component.IsAlive())
                component = gameObject.AddComponent(type);

            return component;
        }

        public static IEnumerable<T> EnumerateComponents<T>(this GameObject gameObject, bool multicomponent = true)
            where T : Component
        {
            if (!multicomponent)
            {
                var result = gameObject.GetComponent<T>();

                if (result.IsAlive()) yield return result;

                yield break;
            }

            foreach (var result in gameObject.GetComponents<T>())
            {
                if (result.IsAlive()) yield return result;
            }
        }

        public static IEnumerable<Component> EnumerateComponents(this GameObject gameObject, System.Type type,
            bool multicomponent = true)
        {
            if (!multicomponent)
            {
                var result = gameObject.GetComponent(type);

                if (result.IsAlive()) yield return result;

                yield break;
            }

            foreach (var result in gameObject.GetComponents(type))
            {
                if (result.IsAlive()) yield return result;
            }
        }

        /// <summary>
        /// Enumerates over the components of specified type in the given enumerable.
        /// Note: there is no check for uniqueness, so the same component might be returned multiple times.
        /// </summary>
        public static IEnumerable<T> ToComponents<T>(this IEnumerable<GameObject> gameObjects,
            bool multicomponent = true) where T : Component
        {
            foreach (var gameObject in gameObjects)
            {
                if (!gameObject.IsAlive()) continue;

                foreach (var result in gameObject.EnumerateComponents<T>(multicomponent))
                {
                    if (result.IsAlive()) yield return result;
                }
            }
        }

        public static void EnableAllComponents(this GameObject gameObject, bool state, bool recursive = false,
            bool nonBehaviors = true, bool behaviors = true)
        {
            if (!(behaviors | nonBehaviors)) return;

            var anyComponent = behaviors & nonBehaviors;

            foreach (var component in gameObject.GetComponents<Component>())
            {
                if (anyComponent | ((component is Behaviour) == behaviors))
                    component.SetEnabled(state);
            }

            if (!recursive) return;

            foreach (Transform child in gameObject.transform)
                child.gameObject.EnableAllComponents(state, true, nonBehaviors, behaviors);
        }

        public static Mesh GetRendererMesh(this GameObject gameObject)
        {
            var skinnedRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();

            if (skinnedRenderer) return skinnedRenderer.sharedMesh;

            var meshFilter = gameObject.GetComponent<MeshFilter>();

            if (meshFilter) return meshFilter.sharedMesh;

            return null;
        }

        public static void SetRendererMesh(this GameObject gameObject, Mesh mesh)
        {
            var skinnedRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();

            if (skinnedRenderer)
            {
                skinnedRenderer.sharedMesh = mesh;
                return;
            }

            var meshFilter = gameObject.GetComponent<MeshFilter>();

            if (!meshFilter) return;

            meshFilter.sharedMesh = mesh;
        }

        /// <summary>
        /// Invoke the callback for components of T type in the GameObject or any of its children.
        /// Uses GetComponentsInChildren(true) to get all components.
        /// </summary>
        public static void ForComponentsInChildren<T>(this GameObject gameObject, System.Action<T> callback)
        {
            foreach (var component in gameObject.GetComponentsInChildren<T>(true))
                callback(component);
        }

        /// <summary>
        /// Invoke the callback for components of T type in the GameObject or any of its parents.
        /// Uses GetComponentsInParent(true) to get all components.
        /// </summary>
        public static void ForComponentsInParents<T>(this GameObject gameObject, System.Action<T> callback)
        {
            foreach (var component in gameObject.GetComponentsInParent<T>(true))
                callback(component);
        }

        /// <summary>
        /// Recursive search by name among the object's children (and the object itself, by default)
        /// </summary>
        public static GameObject FindChild(this GameObject gameObject, string name)
        {
            if (name == gameObject.name) return gameObject;

            foreach (Transform child in gameObject.transform)
            {
                if (!child.IsAlive()) continue;

                var foundChild = child.gameObject.FindChild(name);

                if (foundChild.IsAlive())
                    return foundChild;
            }

            return null;
        }

        public static string GetFullName(this GameObject gameObject)
        {
            var stringBuilder = new StringBuilder(gameObject.name);
            var parent = gameObject.transform.parent;
            while (parent != null)
            {
                stringBuilder.Insert(0, parent.name + @"/");
                parent = parent.parent;
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Returns the full hierarchy path of the object
        /// </summary>
        public static string GetHierarchyPath(this GameObject gameObject)
        {
            var stringBuilder = new System.Text.StringBuilder(gameObject.name);
            var parent = gameObject.transform.parent;

            while (parent.IsAlive())
            {
                stringBuilder.Insert(0, parent.name + @"/");
                parent = parent.parent;
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Returns the root game object
        /// </summary>
        public static GameObject GetRoot(this GameObject gameObject)
        {
            return gameObject.transform.root.gameObject;
        }
    }
}
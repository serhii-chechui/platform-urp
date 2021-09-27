using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bini.ToolKit.Core.Unity.Extensions
{
    public static class SceneObjects
    {
        public static IEnumerable<GameObject> ByName(string name, bool includeInactive = true)
        {
            foreach (var @object in All(includeInactive))
            {
                if (@object.name == name) yield return @object;
            }
        }

        public static IEnumerable<GameObject> ByTag(string tag, bool includeInactive = true)
        {
            foreach (var @object in All(includeInactive))
            {
                if (@object.CompareTag(tag)) yield return @object;
            }
        }

        public static T Any<T>(bool includeInactive = true, bool checkOnlyGameObjects = true) where T : Component
        {
            if (checkOnlyGameObjects & !includeInactive) return Object.FindObjectOfType<T>();

            T first = null;

            foreach (var component in All<T>(includeInactive))
            {
                var isEnabled = checkOnlyGameObjects
                    ? component.gameObject.activeInHierarchy
                    : component.IsEnabled(true);

                if (isEnabled) return component;

                if (!first)
                    first = component;
            }

            return first;
        }

        public static IEnumerable<T> Roots<T>(bool includeInactive = true, bool multicomponent = true)
            where T : Component
        {
            foreach (var gameObject in Roots(includeInactive))
            {
                foreach (var component in gameObject.EnumerateComponents<T>(multicomponent))
                {
                    if (component.IsAlive()) yield return component;
                }
            }
        }

        public static IEnumerable<GameObject> Roots(bool includeInactive = true)
        {
            // At some point scene.GetRootGameObjects() was throwing an exception
            // if the scene was not considered "loaded" (e.g. during Awake/OnEnable).
            // However, it seems that this is fixed now.

            for (var index = 0; index < SceneManager.sceneCount; index++)
            {
                var scene = SceneManager.GetSceneAt(index);

                foreach (var gameObject in scene.GetRootGameObjects())
                {
                    if (!gameObject.IsAlive()) continue;

                    if (includeInactive | gameObject.activeSelf) yield return gameObject;
                }
            }
        }

        public static IEnumerable<T> All<T>(bool includeInactive = true) where T : Component
        {
            return includeInactive ? EnumerateAll<T>() : EnumerateActive<T>();
        }

        public static IEnumerable<GameObject> All(bool includeInactive = true)
        {
            return includeInactive ? EnumerateAll<GameObject>() : EnumerateActive<GameObject>();
        }

        private static IEnumerable<T> EnumerateActive<T>() where T : Object
        {
            foreach (var @object in Object.FindObjectsOfType<T>())
            {
                if (@object.IsAlive()) yield return @object;
            }
        }

        private static IEnumerable<T> EnumerateAll<T>() where T : Object
        {
            const HideFlags skipFlags = HideFlags.NotEditable | HideFlags.HideAndDontSave;

            foreach (var @object in Resources.FindObjectsOfTypeAll<T>())
            {
                if (!@object.IsAlive()) continue;

                if ((@object.hideFlags & skipFlags) != HideFlags.None) continue;

#if UNITY_EDITOR
                if (PrefabUtility.IsPartOfPrefabAsset(@object)) continue;
#endif

                yield return @object;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using Bini.ToolKit.Core.Unity.Utilities.Mesh;
using UnityEditor;
using UnityEngine;

namespace Bini.ToolKit.Core.Unity.Extensions
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Recursive depth-first search by name among the transform's children.
        /// The transform itself is also included in the search by default.
        /// </summary>
        public static Transform FindRecursive(this Transform transform, string name, bool includeSelf = true)
        {
            if (includeSelf && (transform.name == name)) return transform;

            foreach (Transform child in transform)
            {
                var result = FindRecursive(child, name, true);

                if (result.IsAlive()) return result;
            }

            return null;
        }

        public static Transform FindPartial(this Transform selfTransform, string part, bool depthFirst = false)
        {
            var isPath = part.Contains("/");
            return selfTransform.WalkHierarchy(transform =>
                {
                    if (transform == selfTransform)
                        return false; // ignore parent

                    var fullPath = isPath ? transform.GetHierarchyPath(selfTransform.parent) + "/" : transform.name;
                    return fullPath.Contains(part);
                },
                depthFirst);
        }

        public static bool HasParent(this Transform transform, Transform parent, bool checkSelf = false)
        {
            if (checkSelf && transform == parent)
                return true;

            while (true)
            {
                transform = transform.parent;
                if (transform == parent)
                    return true;
                if (!transform)
                    return false;
            }
        }

        public static string GetHierarchyPath(this Transform transform, Transform root = null)
        {
            if (transform == root)
                return string.Empty;

            var hierarchyPath = new StringBuilder(transform.name);
            while (true)
            {
                transform = transform.parent;

                if (transform == root)
                    return hierarchyPath.ToString();

                // Return null if transform is not a child of root at all
                if (!transform.IsAlive())
                    return null;

                hierarchyPath.Insert(0, transform.name + "/");
            }
        }

        public static bool IsSelectedInHierarchy(this Transform transform, bool checkSelf = true)
        {
#if UNITY_EDITOR
            var activeGameObject = Selection.activeGameObject;

            if (!activeGameObject.IsAlive()) return false;

            return activeGameObject.transform.HasParent(transform, checkSelf);
#else
			return false;
#endif
        }

        public static Matrix4x4 GetLocalToObjectMatrix(this Transform transform, Transform other)
        {
            if (!other.IsAlive()) return transform.localToWorldMatrix;

            return other.worldToLocalMatrix * transform.localToWorldMatrix;
        }

        public static Matrix4x4 GetLocalToParentMatrix(this Transform transform)
        {
            return Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
        }

        public static Matrix4x4 GetObjectToLocalMatrix(this Transform transform, Transform other)
        {
            if (!other.IsAlive()) return transform.localToWorldMatrix;

            return transform.worldToLocalMatrix * other.localToWorldMatrix;
        }

        public static Matrix4x4 GetParentToLocalMatrix(this Transform transform)
        {
            return transform.GetLocalToParentMatrix().inverse;
        }

        public static void RecursiveSetLayer(this Transform transform, string layerName)
        {
            transform.RecursiveSetLayer(LayerMask.NameToLayer(layerName));
        }

        public static void RecursiveSetLayer(this Transform transform, int layer)
        {
            transform.gameObject.layer = layer;
            var childCount = transform.childCount;
            for (var i = 0; i < childCount; i++)
                transform.GetChild(i).RecursiveSetLayer(layer);
        }

        public static void RecursiveSetTag(this Transform transform, string tag)
        {
            transform.tag = tag;
            var childCount = transform.childCount;
            for (var i = 0; i < childCount; i++)
                transform.GetChild(i).RecursiveSetTag(tag);
        }

        public static void ResetToParent(this Transform transform, Transform parent, Vector3 position = default)
        {
            transform.ResetToParent(parent, position, Quaternion.identity, Vector3.one);
        }

        public static void ResetToParent(this Transform transform, Transform parent, Vector3 position, Vector3 scale)
        {
            transform.ResetToParent(parent, position, Quaternion.identity, scale);
        }

        public static void ResetToParent(this Transform transform, Transform parent,
            Vector3 position, Quaternion rotation, Vector3 scale)
        {
            if (transform.parent != parent)
                transform.SetParent(parent, false);

            transform.localPosition = position;
            transform.localRotation = rotation;
            transform.localScale = scale;
        }

        public static int UpdateBounds(this Transform transform,
            ref Bounds bounds,
            bool useVertices = true,
            bool includeSelf = true)
        {
            var count = 0;
            transform.UpdateBounds(transform.worldToLocalMatrix, ref bounds, ref count, useVertices, includeSelf);
            return count;
        }

        public static int UpdateBounds(this Transform transform,
            Matrix4x4 matrix,
            ref Bounds bounds,
            bool useVertices = true,
            bool includeSelf = true)
        {
            var count = 0;
            transform.UpdateBounds(matrix, ref bounds, ref count, useVertices, includeSelf);
            return count;
        }

        public static void UpdateBounds(this Transform transform,
            Matrix4x4 matrix,
            ref Bounds bounds,
            ref int count,
            bool useVertices = true,
            bool includeSelf = true)
        {
            if (includeSelf)
            {
                var renderer = transform.GetComponent<Renderer>();
                if (renderer as MeshRenderer)
                {
                    var meshFilter = transform.GetComponent<MeshFilter>();
                    if (meshFilter)
                        meshFilter.UpdateBounds(matrix, ref bounds, ref count, useVertices);
                }
                else if (renderer as SkinnedMeshRenderer)
                    (renderer as SkinnedMeshRenderer).UpdateBounds(matrix, ref bounds, ref count, useVertices);
                else if (renderer as SpriteRenderer)
                    (renderer as SpriteRenderer).UpdateBounds(matrix, ref bounds, ref count, useVertices);
                else
                {
                    var rectTransform = transform.GetComponent<RectTransform>();
                    if (rectTransform)
                        rectTransform.UpdateBounds(matrix, ref bounds, ref count);
                }
            }

            for (var i = 0; i < transform.childCount; i++)
                transform.GetChild(i).UpdateBounds(matrix, ref bounds, ref count, useVertices);
        }

        public static void UpdateBounds(this RectTransform rtfm, Matrix4x4 matrix, ref Bounds bounds, ref int count)
        {
            var vIter = MeshUtils.IterateTransformedBoundingBoxCorners(rtfm, matrix);

            foreach (var v in vIter)
            {
                if (count == 0)
                    bounds = new Bounds(v, Vector3.zero);
                else
                    bounds.Encapsulate(v);

                ++count;
            }
        }

        public static Transform WalkHierarchy<T>(this Transform selfTransform,
            Func<T, bool> action,
            bool depthFirst = true,
            bool allComponents = false) where T : Component
        {
            return selfTransform.WalkHierarchy(transform =>
                {
                    if (allComponents)
                    {
                        var cs = transform.GetComponents<T>();
                        if (cs == null)
                            return false;

                        for (var i = 0; i < cs.Length; i++)
                        {
                            var c = cs[i];
                            if (c && action(c))
                                return true;
                        }
                    }
                    else
                    {
                        var c = transform.GetComponent<T>();
                        if (c && action(c))
                            return true;
                    }

                    return false;
                },
                depthFirst);
        }

        public static Transform WalkHierarchy(this Transform selfTransform,
            Func<Transform, bool> action,
            bool depthFirst = true)
        {
            if (depthFirst)
            {
                var stack = new Stack<Transform>();
                stack.Push(selfTransform);
                while (stack.Count != 0)
                {
                    selfTransform = stack.Pop();
                    if (action(selfTransform))
                        return selfTransform;

                    // Reverse order to match the recursive/FIFO order
                    for (var i = selfTransform.childCount - 1; i != -1; i--)
                        stack.Push(selfTransform.GetChild(i));
                }
            }
            else
            {
                var queue = new Queue<Transform>();
                queue.Enqueue(selfTransform);
                while (queue.Count != 0)
                {
                    selfTransform = queue.Dequeue();
                    if (action(selfTransform))
                        return selfTransform;

                    for (var i = 0; i < selfTransform.childCount; i++)
                        queue.Enqueue(selfTransform.GetChild(i));
                }
            }

            return null;
        }

        public static void WalkHierarchy<T>(this Transform selfTransform,
            Action<T> action,
            bool depthFirst = true,
            bool allComponents = false) where T : Component
        {
            selfTransform.WalkHierarchy(transform =>
                {
                    if (allComponents)
                    {
                        var cs = transform.GetComponents<T>();
                        if (cs == null)
                            return;

                        for (var i = 0; i < cs.Length; i++)
                        {
                            var c = cs[i];
                            if (c)
                                action(c);
                        }
                    }
                    else
                    {
                        var c = transform.GetComponent<T>();
                        if (c)
                            action(c);
                    }
                },
                depthFirst);
        }

        public static void WalkHierarchy(this Transform selfTransform,
            Action<Transform> action,
            bool depthFirst = true)
        {
            if (depthFirst)
            {
                var stack = new Stack<Transform>();
                stack.Push(selfTransform);
                while (stack.Count != 0)
                {
                    selfTransform = stack.Pop();
                    action(selfTransform);
                    // Reverse order to match the recursive/FIFO order
                    for (var i = selfTransform.childCount - 1; i != -1; i--)
                        stack.Push(selfTransform.GetChild(i));
                }
            }
            else
            {
                var queue = new Queue<Transform>();
                queue.Enqueue(selfTransform);
                while (queue.Count != 0)
                {
                    selfTransform = queue.Dequeue();
                    action(selfTransform);
                    for (var i = 0; i < selfTransform.childCount; i++)
                        queue.Enqueue(selfTransform.GetChild(i));
                }
            }
        }
    }
}
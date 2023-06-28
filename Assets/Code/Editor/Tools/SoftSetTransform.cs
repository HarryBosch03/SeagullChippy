using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ShootingRangeGameEditor.Tools
{
    public static class SoftSetTransform
    {
        [MenuItem("Tools/Actions/Soft Set Transform")]
        public static void Apply()
        {
            foreach (var gameObject in Selection.gameObjects)
            {
                var transform = gameObject.transform;
                var children = new List<Transform>();
                foreach (Transform child in transform)
                {
                    children.Add(child);
                }

                foreach (var child in children)
                {
                    child.SetParent(null, true);
                }

                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                transform.localScale = Vector3.one;

                foreach (var child in children)
                {
                    child.SetParent(transform, true);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using ShootingRangeGame.AI.BehaviourTrees.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ShootingRangeGameEditor.Tools.Windows
{
    public class TreeInspector : EditorWindow
    {
        [MenuItem("Window/Analysis/Behaviour Tree Inspector", false, -100)]
        public static void OpenWindow()
        {
            CreateWindow<TreeInspector>("Tree Inspector");
        }

        private IHasBehaviourTree selected;

        private void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true)))
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.MaxWidth(250), GUILayout.ExpandHeight(true)))
                {
                    GUILayout.Label("Selection", EditorStyles.boldLabel);
                    var queries = Selection.gameObjects;
                    var hasTrees = new List<IHasBehaviourTree>();
                    foreach (var query in queries)
                    {
                        if (query.TryGetComponent(out IHasBehaviourTree tree))
                        {
                            if (hasTrees.Contains(tree)) continue;
                            hasTrees.Add(tree);
                        }
                    }

                    hasTrees.Sort((a, b) => string.Compare(a.Behaviour.name, b.Behaviour.name));

                    foreach (var behaviour in hasTrees)
                    {
                        if (selected == behaviour) GUI.enabled = false;
                        if (GUILayout.Button(behaviour.Behaviour.name, GUILayout.Height(30)))
                        {
                            selected = behaviour;
                            Selection.objects = new Object[] { behaviour.Behaviour.gameObject };
                        }

                        GUI.enabled = true;
                    }

                    GUILayout.Space(10);
                    if (GUILayout.Button("Deselect", GUILayout.Height(30)))
                    {
                        selected = null;
                    }
                }

                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.ExpandHeight(true)))
                {
                    if (selected == null)
                    {
                        GUILayout.Label("Nothing Selected", EditorStyles.boldLabel);
                    }
                    else
                    {
                        GUILayout.Label(selected.Behaviour.name, EditorStyles.boldLabel);
                        DrawTree();
                    }
                }
            }

            Repaint();
        }

        private bool isRoot;

        private void DrawTree()
        {
            var name = selected.Behaviour.name;
            isRoot = true;

            if (selected.Tree == null)
            {
                EditorGUILayout.HelpBox($"{name} contains a null tree", MessageType.Error);
                return;
            }

            if (selected.Tree.root == null)
            {
                EditorGUILayout.HelpBox($"{name}'s Tree does not contain a Root Leaf", MessageType.Error);
                return;
            }

            selected.Tree.root.RecursiveAction(selected.Tree, DrawLeaf);
            EditorGUI.indentLevel = 0;
        }

        private void DrawLeaf(LeafContext context)
        {
            EditorGUI.indentLevel = context.depth;

            GUI.backgroundColor = context.leaf.LastEvaluationResult switch
            {
                BehaviourTree.Result.Success => Color.green,
                BehaviourTree.Result.Failure => Color.red,
                BehaviourTree.Result.Pending => Color.yellow,
                _ => throw new ArgumentOutOfRangeException()
            };
            var fresh = context.leaf.LastEvaluatedFrame == Time.frameCount;
            if (!fresh) GUI.backgroundColor = Color.grey;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.ExpandWidth(true)))
            {
                EditorGUILayout.LabelField(isRoot ? $"[ROOT]{context.leaf.Name}" : context.leaf.Name, EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField(fresh ? $"Execution Results: {context.leaf.LastEvaluationResult}" : "--- Did Not Execute ---");

                EditorGUI.indentLevel--;
                isRoot = false;
            }
        }
    }
}
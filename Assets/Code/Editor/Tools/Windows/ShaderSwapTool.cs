using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ShootingRangeGameEditor.Editor.Tools.Windows
{
    public sealed class ShaderSwapTool : EditorWindow
    {
        private Shader from, to;

        private Vector2 scrollPos;

        private void OnGUI()
        {
            from = EditorGUILayout.ObjectField("Change Shader From", from, typeof(Shader), false) as Shader;
            to = EditorGUILayout.ObjectField("Change Shader To", to, typeof(Shader), false) as Shader;

            var materials = GetMaterials();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Clear"))
                {
                    from = null;
                    to = null;
                }

                if (GUILayout.Button("Swap To and From"))
                {
                    var a = to;
                    var b = from;

                    to = b;
                    from = a;
                }
            }
            
            EditorGUILayout.HelpBox("Swapping materials is not supported by Unity's undo system.\nIF YOU USE THIS, IT CANNOT BE UNDONE, MAKE SURE YOUR PROJECT IS BACKED UP THROUGH GIT\nIF YOU ARE AN ARTIST AND THIS MESSAGE CONFUSES YOU, DO NOT USE THIS TOOL", MessageType.Error);
            
            if (GUILayout.Button("--- Swap ---"))
            {
                SwapShaders(materials);
            }

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                
                using (new EditorGUILayout.VerticalScope())
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        foreach (var m in materials)
                        {
                            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                            {
                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    using (new EditorGUILayout.VerticalScope())
                                    {
                                        GUILayout.Label(m.name, EditorStyles.boldLabel);
                                        using (new EditorGUI.IndentLevelScope())
                                        {
                                            GUILayout.Label($"Shader: \"{m.shader.name}\"");
                                        }
                                    }


                                    if (GUILayout.Button("Set From", GUILayout.Width(70), GUILayout.ExpandHeight(true)))
                                    {
                                        from = m.shader;
                                    }

                                    if (GUILayout.Button("Set To", GUILayout.Width(70), GUILayout.ExpandHeight(true)))
                                    {
                                        to = m.shader;
                                    }

                                    if (GUILayout.Button("Show In Project", GUILayout.Width(130), GUILayout.ExpandHeight(true)))
                                    {
                                        EditorGUIUtility.PingObject(m);
                                        Selection.objects = new Object[] { m };
                                    }
                                }
                            }
                        }
                    }
                }

                EditorGUILayout.EndScrollView();
            }
        }

        private void SwapShaders(List<Material> materials)
        {
            if (!to)
            {
                Debug.LogWarning($"Shader \"{to}\" does not exist");
                return;
            }

            var c = 0;
            foreach (var m in materials)
            {
                if (m.shader != from) continue;
                
                m.shader = to;
                c++;
            }

            Debug.Log($"Done! {c} Shaders modified.");
        }

        private List<Material> GetMaterials()
        {
            var list = new List<Material>();
            var guids = AssetDatabase.FindAssets("t:material");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path[.."Assets".Length] != "Assets") continue;

                var material = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (material.shader.name[.."GUI/".Length] == "GUI/") continue;
                list.Add(material);
            }

            return list;
        }

        private string Simplify(string text) => text.ToLower().Trim();

        [MenuItem("Tools/Shader Swap Tool")]
        public static void Open()
        {
            CreateWindow<ShaderSwapTool>(nameof(ShaderSwapTool));
        }
    }
}
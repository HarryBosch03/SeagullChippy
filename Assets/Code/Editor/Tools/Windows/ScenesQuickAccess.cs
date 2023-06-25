using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ShootingRangeGameEditor.Tools.Windows
{
    public class ScenesQuickAccess : EditorWindow
    {
        [MenuItem("Tools/Custom/Scene Quick Access")]
        public static void Open()
        {
            CreateWindow<ScenesQuickAccess>("Scenes");
        }

        private void OnGUI()
        {
            var scenes = GetScenes();
            var icon = EditorGUIUtility.IconContent("d_SceneAsset Icon").image;
            foreach (var scene in scenes)
            {
                var content = new GUIContent($"Open {scene.name}", icon);
                content.tooltip = $"Save current scene and Load \"{scene.name}.unity\"";
                if (GUILayout.Button(content, GUILayout.Height(40)))
                {
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    var path = AssetDatabase.GetAssetPath(scene);
                    EditorSceneManager.OpenScene(path);
                }
            }
        }

        private IEnumerable<SceneAsset> GetScenes()
        {
            var scenes = new List<SceneAsset>();
            var guids = AssetDatabase.FindAssets("t:scene");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("/Unity/")) continue;
                if (path.Contains("Packages/")) continue;
                
                var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                scenes.Add(asset);
            }
            return scenes;
        }
    }
}
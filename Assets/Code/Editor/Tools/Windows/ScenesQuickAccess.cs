using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ShootingRangeGameEditor.Tools.Windows
{
    public class ScenesQuickAccess : EditorWindow
    {
        private Vector2 scrollPos;

        private List<SceneAsset> cache;
        
        [MenuItem("Tools/Custom/Scene Quick Access")]
        public static void Open()
        {
            CreateWindow<ScenesQuickAccess>("Scenes");
        }

        private void Awake()
        {
            cache = new List<SceneAsset>();
            GetScenes(ref cache);
        }

        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUIStyle.none, GUI.skin.verticalScrollbar);
        
            var sceneIcon = EditorGUIUtility.IconContent("d_SceneAsset Icon").image;
            foreach (var scene in cache)
            {
                var content = new GUIContent($" Open {scene.name}", sceneIcon);
                content.tooltip = $"Save current scene and Load \"{scene.name}.unity\"";
                if (GUILayout.Button(content, GUILayout.Height(40)))
                {
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    var path = AssetDatabase.GetAssetPath(scene);
                    EditorSceneManager.OpenScene(path);
                }
            }
            
            EditorGUILayout.EndScrollView();
            
            var refreshIcon = EditorGUIUtility.IconContent("d_Refresh").image;
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 0.4f);
            if (GUILayout.Button(new GUIContent(" Refresh", refreshIcon)))
            {
                GetScenes(ref cache);
            }
        }

        private void GetScenes(ref List<SceneAsset> scenes)
        {
            scenes.Clear();
            
            var guids = AssetDatabase.FindAssets("t:scene");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("/Unity/")) continue;
                if (path.Contains("Packages/")) continue;
                
                var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                scenes.Add(asset);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ShootingRangeGameEditor.Tools.Windows
{
    public class ScenesQuickAccess : EditorWindow
    {
        private Vector2 scrollPos;
        private string filter;
        private bool dirty = true;

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
            if (dirty)
            {
                GetScenes(ref cache);
                dirty = false;
            }
            
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

            var tmp = EditorGUILayout.TextField("Filter [regex]", filter);
            if (filter != tmp) SetDirty();
            filter = tmp;
            
            var refreshIcon = EditorGUIUtility.IconContent("d_Refresh").image;
            if (GUILayout.Button(new GUIContent(" Refresh", refreshIcon)))
            {
                SetDirty();
            }
        }

        private void GetScenes(ref List<SceneAsset> scenes)
        {
            scenes.Clear();

            var filterRegex = new Regex(filter, RegexOptions.IgnoreCase);
            
            var guids = AssetDatabase.FindAssets("t:scene");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("/Unity/")) continue;
                if (path.Contains("Packages/")) continue;
                if (!filterRegex.IsMatch(Path.GetFileNameWithoutExtension(path))) continue; 
                
                var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                scenes.Add(asset);
            }
        }

        public void SetDirty()
        {
            dirty = true;
            Repaint();
        }
    }
}
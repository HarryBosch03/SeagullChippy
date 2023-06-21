using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ShootingRangeGame.Tools
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class VistaCamera : MonoBehaviour
    {
        public string ScreenshotDirectory => $"{Application.dataPath}/~Screenshots/";

        public string filename = "Screenshot - <timestamp>";
        public int supersize = 2;

        private void Reset()
        {
            SetCameraState(false);
        }

        public void Capture()
        {
            SetGuides(false);
            
            var filename = $"{this.filename}.png";

            filename = filename.Replace("<timestamp>", DateTime.Now.ToString("yyyy-mm-dd.hh.mm.ss tt"));
            
            filename = $"{ScreenshotDirectory}{filename}";
            var directory = Path.GetDirectoryName(filename);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            
            ScreenCapture.CaptureScreenshot(filename, supersize);
            StartCoroutine(WaitUntilScreenshotIsTaken(filename, () =>
            {
                var assetPath = filename[(Application.dataPath.Length - "Assets/".Length + 1)..];

                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                Selection.objects = new[] { asset };
                
                EditorGUIUtility.PingObject(asset);
            
                Debug.Log($"Saved Screenshot \"{Path.GetFileName(filename)}\" at \"{Path.GetDirectoryName(filename)}\"");

                SetGuides(true);
            }));
        }

        private IEnumerator WaitUntilScreenshotIsTaken(string filename, Action callback)
        {
            yield return new WaitUntil(() => File.Exists(filename));
            callback();
        }

        public void SetCameraState(bool state)
        {
            var camera = GetComponent<Camera>();
            camera.enabled = state;
            camera.depth = state ? 999 : -999;
        }

        public void SetGuides(bool state)
        {
            var guides = transform.Find("Guides");
            if (!guides) return;
            guides.gameObject.SetActive(state);
        }
    }
}

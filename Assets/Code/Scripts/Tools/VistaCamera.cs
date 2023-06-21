using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ShootingRangeGame.Tools
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class VistaCamera : MonoBehaviour
    {
        public string ScreenshotDirectory => $"{Application.dataPath}/Screenshots~/";

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

            Debug.Log($"Saved Screenshot \"{Path.GetFileName(filename)}\" at \"{Path.GetDirectoryName(filename)}\"");
            SetGuides(true);

            Process.Start("explorer.exe", $"\"{directory}\"");
;        }

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
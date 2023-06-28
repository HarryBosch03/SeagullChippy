using TMPro;
using UnityEngine;

namespace ShootingRangeGame.UI
{
    [System.Serializable]
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text msCounter, fpsCounter;

        private string msTemplate, fpsTemplate;
        private int smoothedFramerate;
        private float smoothedFrameTime;

        private float DeltaTime => Time.smoothDeltaTime;
        
        private void Start()
        {
            msTemplate = msCounter.text;
            fpsTemplate = fpsCounter.text;
        }

        private void Update()
        {
            var framerate = Mathf.Round(1.0f / DeltaTime);
            smoothedFramerate = Mathf.RoundToInt((smoothedFramerate + framerate) / 2.0f);

            var frameTime = DeltaTime;
            smoothedFrameTime = (smoothedFrameTime + frameTime) / 2.0f;

            msCounter.text = string.Format(msTemplate, Mathf.RoundToInt(smoothedFrameTime * 1000.0f));
            fpsCounter.text = string.Format(fpsTemplate, smoothedFramerate);
        }
    }
}

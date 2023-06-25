using ShootingRangeGame.Tools;
using UnityEditor;
using UnityEngine;

namespace ShootingRangeGameEditor.Inspectors
{
    [CustomEditor(typeof(VistaCamera))]
    public class VistaCameraEditor : Editor<VistaCamera>
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Capture"))
            {
                Target.Capture();
            }
        }

        private void OnEnable()
        {
            Target.SetCameraState(true);
        }

        private void OnDisable()
        {
            Target.SetCameraState(false);
        }
    }
}
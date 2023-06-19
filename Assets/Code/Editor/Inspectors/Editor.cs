using UnityEditor;
using UnityEngine;

namespace ShootingRangeGame.Editor.Inspectors
{
    public class Editor<T> : UnityEditor.Editor where T : Object
    {
        public T Target => (T)target;

        protected void Div()
        {
            var rect = EditorGUILayout.GetControlRect();
            rect.y += rect.height / 2.0f;
            rect.y = 1.0f;
            EditorGUI.DrawRect(rect, new Color(1.0f, 1.0f, 1.0f, 0.1f));
        }
    }
}
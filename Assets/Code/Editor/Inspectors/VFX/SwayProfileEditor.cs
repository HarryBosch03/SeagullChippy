using System;
using ShootingRangeGame.VFX;
using UnityEditor;
using UnityEngine;

namespace ShootingRangeGameEditor.Inspectors.VFX
{
    [CustomEditor(typeof(SwayProfile))]
    public class SwayProfileEditor : Editor<SwayProfile>
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Div();

            var rect = EditorGUILayout.GetControlRect(false, 120.0f);
            const float shadowSize = 2.0f;
            const float shadowDrop = 2.0f;

            EditorGUI.DrawRect(rect, new Color(0.0f, 0.0f, 0.0f, 0.1f));

            rect.x += shadowSize;
            rect.y += shadowSize;
            rect.width -= 2.0f * shadowSize;
            rect.height -= 2.0f * shadowDrop * shadowSize;

            GUI.BeginClip(rect);
            GL.PushMatrix();
            GL.Clear(true, false, Color.black);

            GL.Begin(GL.QUADS);
            GL.Color(Color.black);
            GL.Vertex3(0.0f, 0.0f, 0.0f);
            GL.Vertex3(rect.width, 0.0f, 0.0f);
            GL.Vertex3(rect.width, rect.height, 0.0f);
            GL.Vertex3(0.0f, rect.height, 0.0f);
            GL.End();

            var profile = target as SwayProfile;
            const float duration = 5.0f;
            const float step = 0.02f;

            GL.Begin(GL.LINES);
            GL.Color(new Color(0.0f, 1.0f, 0.5f, 1.0f));
            
            for (var t = 0.0f; t < duration; t += step)
            {
                var t2 = t + step;

                graph(t, t2);
            }

            GL.End();

            GL.PopMatrix();
            GUI.EndClip();

            Repaint();

            void graph(float t1, float t2)
            {
                var diff = (t1 % duration + duration) % duration - t1;

                t1 += diff;
                t2 += diff;

                var n1 = profile.GetNoise(Time.time + t1 - 1.0f, 0.0f);
                var n2 = profile.GetNoise(Time.time + t2 - 1.0f, 0.0f);

                GL.Vertex3(t1 / duration * rect.width, n1 * rect.height, 0.0f);
                GL.Vertex3(t2 / duration * rect.width, n2 * rect.height, 0.0f);
            }
        }
    }
}
using UnityEngine;

namespace ShootingRangeGame
{
    public static class Extensions
    {
        public static Color Normalized(this Color color)
        {
            var m = Mathf.Max(color.r, color.g, color.b);
            if (m == 0.0f) return Color.black;
            return new Color
            {
                r = color.r / m,
                g = color.g / m,
                b = color.b / m,
                a = color.a,
            };
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            return component ? component : gameObject.AddComponent<T>();
        }

        public static void SetLine(this LineRenderer lines, Vector3 a, Vector3 b)
        {
            lines.enabled = true;
            lines.useWorldSpace = true;

            var start = a;
            var difference = b - a;
            for (var i = 0; i < lines.positionCount; i++)
            {
                var percent = i / (lines.positionCount - 1.0f);
                lines.SetPosition(i, start + difference * percent);
            }
        }
    }
}
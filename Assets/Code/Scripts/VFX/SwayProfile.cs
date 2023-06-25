using UnityEngine;

namespace ShootingRangeGame.VFX
{
    [CreateAssetMenu(menuName = "Scriptable Objects/VFX/Sway")]
    public class SwayProfile : ScriptableObject
    {
        public Vector3 scale = new(1.0f, 0.0f, 0.1f);
        public Vector3 skew = new(0.0f, 0.0f, 0.5f);
        public float frequency = 1.0f;
        public int octaves = 5;
        public float persistence = 0.5f;
        public float lacunarity = 2.0f;
        public float variance = 0.1f;

        public Quaternion GetRotation(float offset, float weight = 1.0f)
        {
            var noise = GetNoise(Time.time, offset);
            return Quaternion.Euler(scale * noise * weight - Vector3.Scale(scale * 0.5f, skew));
        }
        
        public float GetNoise(float position, float offset)
        {
            var val = 0.0f;
            var max = 0.0f;

            for (var i = 0; i < octaves; i++)
            {
                var f = frequency * Mathf.Pow(lacunarity, i);
                var a = Mathf.Pow(persistence, i);

                val += Mathf.PerlinNoise((position + offset * variance) * f, 0.5f) * a;
                max += a;
            }
            
            return val / max;
        }
    }
}
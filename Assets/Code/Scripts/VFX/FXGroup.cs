using System;
using UnityEngine;

namespace ShootingRangeGame.VFX
{
    [Serializable]
    public class FXGroup : MonoBehaviour
    {
        private float lifetime;

        private void Awake()
        {
            lifetime = GetLifetime();

            gameObject.SetActive(false);
        }

        private float GetLifetime()
        {
            var lifetime = 1.0f;

            void getLifetimeFromComponent<T>(Func<T, float> callback)
            {
                var components = GetComponentsInChildren<T>();
                foreach (var component in components)
                {
                    lifetime = Mathf.Max(lifetime, callback(component));
                }
            }

            getLifetimeFromComponent<ParticleSystem>(c =>
            {
                float maxFromCurve(AnimationCurve curve)
                {
                    var max = 0.0f;
                    foreach (var key in curve.keys) max = Mathf.Max(key.value, max);
                    return max;
                }

                var lifetime = c.main.startLifetime;
                return lifetime.mode switch
                {
                    ParticleSystemCurveMode.Constant => lifetime.constant,
                    ParticleSystemCurveMode.Curve => maxFromCurve(lifetime.curve),
                    ParticleSystemCurveMode.TwoCurves => Mathf.Max(maxFromCurve(lifetime.curveMin), maxFromCurve(lifetime.curveMax)),
                    ParticleSystemCurveMode.TwoConstants => Mathf.Max(lifetime.constantMin, lifetime.constantMax),
                    _ => throw new ArgumentOutOfRangeException()
                };
            });
            getLifetimeFromComponent<AudioSource>(c => c.clip.length);

            return lifetime + 1.0f;
        }

        public static void Try(FXGroup group, Action<FXGroup> callback)
        {
            if (group) callback(group);
        }

        public FXGroup Instance()
        {
            var instance = Instantiate(this);
            return instance;
        }

        public FXGroup At(GameObject gameObject) => At(gameObject.transform);
        public FXGroup At(Component behaviour) => At(behaviour.transform);
        public FXGroup At(Transform transform) => At(transform.position, transform.rotation);

        public FXGroup At(Vector3 position, Quaternion orientation)
        {
            transform.position = position;
            transform.rotation = orientation;
            return this;
        }

        public FXGroup WithSize(float scale)
        {
            transform.localScale = Vector3.one * scale;
            return this;
        }

        public FXGroup Play()
        {
            gameObject.SetActive(true);
            return this;
        }

        public void AndDestroy()
        {
            Destroy(gameObject, lifetime);
        }
    }
}
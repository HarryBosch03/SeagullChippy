using System;
using System.Collections.Generic;
using ShootingRangeGame.Audio;
using ShootingRangeGame.VFX;
using UnityEngine;

namespace ShootingRangeGame
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class Ocean : MonoBehaviour
    {
        [SerializeField] private FXGroup splashFX;
        [SerializeField] private float splashScale;
        [SerializeField] private AudioClipGroup splashAudio;
        
        private void FixedUpdate()
        {
            var rigidbodies = FindObjectsOfType<Rigidbody>();
            foreach (var rb in rigidbodies)
            {
                var pos = rb.position;
                var lastPos = rb.position - rb.velocity * Time.deltaTime;

                var isInWater = pos.y < transform.position.y;
                var wasInWater = lastPos.y < transform.position.y;

                if (isInWater != wasInWater)
                {
                    var diff = transform.position.y - lastPos.y;
                    var vec = pos - lastPos;
                    vec /= vec.y;
                    var spawnPos = lastPos + vec * diff;

                    var energy = rb.velocity.magnitude * rb.mass;
                    var size = Mathf.Pow(energy, 1.0f / 3.0f) * splashScale;
                    
                    splashFX.Instance().At(spawnPos, Quaternion.identity).WithSize(size).Play().AndDestroy();
                    splashAudio.Play(spawnPos);
                }
            }
        }
    }
}

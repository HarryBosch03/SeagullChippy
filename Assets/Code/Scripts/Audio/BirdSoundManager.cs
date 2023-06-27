using ShootingRangeGame.Seagulls;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShootingRangeGame.Audio
{
    public class BirdSoundManager : MonoBehaviour
    {
        [SerializeField] private int tracks;
        [SerializeField] private Vector2 delayRange;
        [SerializeField] private AudioClipGroup clipGroup;

        private float[] timers;

        private void Awake()
        {
            timers = new float[tracks];
            for (var i = 0; i < timers.Length; i++)
            {
                ref var timer = ref timers[i];
                timer = Time.time + Random.Range(delayRange.x, delayRange.y);
            }
        }

        private void Update()
        {
            for (var i = 0; i < timers.Length; i++)
            {
                if (Time.time < timers[i]) continue;

                timers[i] = Time.time + Random.Range(delayRange.x, delayRange.y);
                PlaySound();
            }
        }

        private void PlaySound()
        {
            var birds = FindObjectsOfType<Bird>();
            if (birds.Length == 0) return;

            var bird = birds[Random.Range(0, birds.Length)];
            clipGroup.Play(bird.transform.position);
        }
    }
}

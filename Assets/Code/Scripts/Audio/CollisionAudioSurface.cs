using UnityEngine;

namespace ShootingRangeGame.Audio
{
    public class CollisionAudioSurface : MonoBehaviour, ICollisionAudioSurface
    {
        [SerializeField] private new AudioClipGroup audio;
        
        public void Play(Vector3 position) => audio.Play(position);
    }
}
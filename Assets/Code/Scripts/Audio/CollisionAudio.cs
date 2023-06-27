using UnityEngine;

namespace ShootingRangeGame.Audio
{
    public class CollisionAudio : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            var surface = (collision.rigidbody ? (Component)collision.rigidbody : collision.transform).GetComponent<ICollisionAudioSurface>();
            if (surface == null) return;
            
            surface.Play(transform.position);
        }
    }
}
using UnityEngine;

namespace ShootingRangeGame.Audio
{
    public class PlayRandomClipOnStart : MonoBehaviour
    {
        [SerializeField] private AudioClipGroup clipGroup;

        private void Start()
        {
            var source = GetComponentInChildren<AudioSource>();
            clipGroup.Play(source, transform.position);
        }
    }
}

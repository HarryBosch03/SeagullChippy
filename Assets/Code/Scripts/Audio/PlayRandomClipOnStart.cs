using System;
using UnityEngine;

namespace ShootingRangeGame.Audio
{
    public class PlayRandomClipOnStart : MonoBehaviour
    {
        [SerializeField] private AudioClipGroup clipGroup;

        private void Start()
        {
            clipGroup.PlayThroughAudioSourceOrAtPointIfAudioSourceDoesNotExist(this);
        }
    }
}
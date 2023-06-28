using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace ShootingRangeGame.Audio
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Audio/Audio Clip Group")]
    public class AudioClipGroup : ScriptableObject
    {
        [SerializeField] private List<AudioClip> list;
        [SerializeField] private Mode mode;
        [SerializeField] private Vector2 volumeRange = Vector2.one;
        [SerializeField] private Vector2 pitchRange = Vector2.one;
        [SerializeField] private AudioSource sourcePrefab;
        [SerializeField] private AudioMixerGroup mixerGroup;

        private int index;

        private static AudioListener listener;

        public void Play(Action<AudioClip> callback)
        {
            if (list.Count == 0) return;

            switch (mode)
            {
                default:
                case Mode.First:
                    callback(list[0]);
                    break;
                case Mode.Sequential:
                    PlaySequential(callback);
                    break;
                case Mode.Random:
                    PlayRandom(callback);
                    break;
            }

            index++;
        }

        public void Play(AudioSource source) => Play(clipEntry =>
        {
            float random(Vector2 range) => Random.Range(range.x, range.y);

            source.volume = random(volumeRange);
            source.pitch = random(pitchRange);
            source.outputAudioMixerGroup = mixerGroup;
            source.PlayOneShot(clipEntry);
        });

        public void Play(Vector3 position) => Play(clipEntry =>
        {
            AudioSource source;
            if (sourcePrefab)
            {
                source = Instantiate(sourcePrefab);
            }
            else
            {
                source = new GameObject().AddComponent<AudioSource>();
                source.spatialize = true;
                source.spatialBlend = 1.0f;
            }

            source.gameObject.name = "[TEMP] Audio Source";
            source.transform.position = position;
            Destroy(source.gameObject, clipEntry.length + 0.5f);

            Play(source);
        });

        public void Play(AudioSource source, Vector3 position)
        {
            if (source) Play(source);
            else Play(position);
        }

        public void Play()
        {
            if (!listener)
            {
                listener = FindObjectOfType<AudioListener>();
                if (!listener) return;
            }

            Play(listener.transform.position);
        }

        private void PlaySequential(Action<AudioClip> callback)
        {
            callback(list[index++ % list.Count]);
        }

        private void PlayRandom(Action<AudioClip> callback)
        {
            callback(list[Random.Range(0, list.Count)]);
        }

        public enum Mode
        {
            First,
            Random,
            Sequential,
        }
    }
}
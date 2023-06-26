using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace ShootingRangeGame.Audio
{
    [Serializable]
    public class AudioClipGroup
    {
        [SerializeField] private List<AudioClipEntry> list;
        [SerializeField] private Mode mode;
        [SerializeField] private int staleCount;

        private int index;

        private static AudioListener listener;

        public void Play(Action<AudioClipEntry> callback)
        {
            switch (mode)
            {
                default:
                case Mode.First:
                    callback(list[0]);
                    break;
                case Mode.Sequential:
                    PlaySequential(GetPool(), callback);
                    break;
                case Mode.Random:
                    PlayRandom(GetPool(), callback);
                    break;
            }

            index++;
        }

        public void Play(AudioSource source) => Play(clipEntry =>
        {
            float random(Vector2 range) => Random.Range(range.x, range.y);

            source.volume = random(clipEntry.volumeRange);
            source.pitch = random(clipEntry.pitchRange);
            source.PlayOneShot(clipEntry.clip);
        });

        public void Play(Vector3 position) => Play(clipEntry =>
        {
            float random(Vector2 range) => Random.Range(range.x, range.y);

            var source = new GameObject("[TEMP] Audio Source").AddComponent<AudioSource>();
            source.transform.position = position;

            source.volume = random(clipEntry.volumeRange);
            source.pitch = random(clipEntry.pitchRange);
            source.PlayOneShot(clipEntry.clip);

            Object.Destroy(source.gameObject, clipEntry.clip.length + 0.5f);
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
                listener = Object.FindObjectOfType<AudioListener>();
                if (!listener) return;
            }

            Play(listener.transform.position);
        }

        public List<AudioClipEntry> GetPool()
        {
            var pool = new List<AudioClipEntry>();
            foreach (var element in list)
            {
                if (index - element.lastPlayedIndex <= staleCount) continue;
                pool.Add(element);
            }

            return pool;
        }

        private void PlaySequential(List<AudioClipEntry> pool, Action<AudioClipEntry> callback)
        {
            callback(pool[index % pool.Count]);
        }

        private void PlayRandom(List<AudioClipEntry> pool, Action<AudioClipEntry> callback)
        {
            var totalWeight = 0.0f;
            foreach (var element in pool)
            {
                totalWeight += element.weight;
            }

            var weight = Random.value * totalWeight;
            foreach (var element in pool)
            {
                if (element.weight < weight)
                {
                    weight -= element.weight;
                    continue;
                }

                callback(element);
                element.lastPlayedIndex = index;
                return;
            }
        }

        [Serializable]
        public class AudioClipEntry
        {
            public string name;
            public AudioClip clip;
            public float weight = 1.0f;
            public Vector2 volumeRange = Vector2.one;
            public Vector2 pitchRange = Vector2.one;
            public int lastPlayedIndex;
        }

        public enum Mode
        {
            First,
            Random,
            Sequential,
        }
    }
}
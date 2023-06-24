using System;
using System.Collections.Generic;
using UnityEngine;
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

        public void Play(Action<AudioClip> callback)
        {
            switch (mode)
            {
                case Mode.Sequential:
                    PlaySequential(GetPool(), callback);
                    break;
                default:
                case Mode.Random:
                    PlayRandom(GetPool(), callback);
                    break;
            }
            index++;
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
        
        private void PlaySequential(List<AudioClipEntry> pool, Action<AudioClip> callback)
        {
            callback(pool[index % pool.Count].clip);
        }
        
        private void PlayRandom(List<AudioClipEntry> pool, Action<AudioClip> callback)
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

                callback(element.clip);
                element.lastPlayedIndex = index;
                return;
            }
        }

        public void PlayThroughAudioSourceOrAtPointIfAudioSourceDoesNotExist(MonoBehaviour caller)
        {
            Action<AudioClip> callback;
            var audioSource = caller.GetComponentInChildren<AudioSource>();
            
            if (audioSource) callback = audioSource.PlayOneShot;
            else callback = clip => AudioSource.PlayClipAtPoint(clip, caller.transform.position);
            
            Play(callback);
        }

        [Serializable]
        public class AudioClipEntry
        {
            public string name;
            public AudioClip clip;
            public float weight;
            public int lastPlayedIndex;
        }

        public enum Mode
        {
            Random,
            Sequential,
        }
    }
}
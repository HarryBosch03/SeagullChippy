using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace ShootingRangeGame.Seagulls
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class SeagullSpawner : MonoBehaviour
    {
        [FormerlySerializedAs("prefab")] 
        [SerializeField] private Bird seagullPrefab;
        [SerializeField] private Bird pigeonPrefab;
        [SerializeField] private int startCount = 20;
        [SerializeField] private int maintainCount = 20;
        [SerializeField] private float checkDelay = 10.0f;
        [SerializeField] private float spawnDelay = 3.0f;
        [SerializeField] private float minRange = 5.0f;
        [SerializeField] private float maxRange = 20.0f;
        [SerializeField] [Range(0.0f, 1.0f)] private float pigeonSeagullRatio = 1.0f;

        private float nextCheckTime;
        private new Camera camera;

        private readonly List<Bird> birds = new();

        private void Awake()
        {
            camera = Camera.main;
        }

        private void Start()
        {
            for (var i = 0; i < startCount; i++)
            {
                SpawnSeagull(true);
            }
        }

        private void Update()
        {
            if (Time.time < nextCheckTime) return;
            birds.RemoveAll(e => !e);

            if (birds.Count >= maintainCount)
            {
                nextCheckTime = Time.time + checkDelay;
                return;
            }

            SpawnSeagull();
            nextCheckTime = Time.time + spawnDelay;
        }

        private void SpawnSeagull(bool ignoreView = false)
        {
            var angle = Random.value * Mathf.PI * 2.0f;
            var distance = Random.value * (maxRange - minRange) + minRange;
            var position = new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle)) * distance;

            var ray = new Ray(position + Vector3.up * 50.0f, Vector3.down);
            if (!Physics.Raycast(ray, out var hit)) return;
            position = hit.point;

            if (!ignoreView)
            {
                var viewPoint = camera.WorldToViewportPoint(position);
                if (viewPoint.x > -1.0f && viewPoint.x < 1.0f) return;
                if (viewPoint.y > -1.0f && viewPoint.y < 1.0f) return;
            }

            var prefab = GetPrefab();

            var instance = Instantiate(prefab, position, Quaternion.Euler(0.0f, Random.value * 360.0f, 0.0f));
            birds.Add(instance);
        }

        private Bird GetPrefab()
        {
            var seagullCount = 0;
            var pigeonCount = 0;

            foreach (var bird in birds)
            {
                switch (bird.Type)
                {
                    case Bird.BirdType.Seagull:
                        seagullCount++;
                        break;
                    case Bird.BirdType.Pigeon:
                        pigeonCount++;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var cRatio = pigeonCount / (float)seagullCount;
            return cRatio > pigeonSeagullRatio ? seagullPrefab : pigeonPrefab;
        }
    }
}
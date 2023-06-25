using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShootingRangeGame.Seagulls
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class SeagullSpawner : MonoBehaviour
    {
        [SerializeField] private Seagull prefab;
        [SerializeField] private int startCount = 20;
        [SerializeField] private int maintainCount = 20;
        [SerializeField] private float checkDelay = 10.0f;
        [SerializeField] private float spawnDelay = 3.0f;
        [SerializeField] private float minRange = 5.0f;
        [SerializeField] private float maxRange = 20.0f;

        private float nextCheckTime;
        private new Camera camera;

        private readonly List<Seagull> seagulls = new();

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
            seagulls.RemoveAll(e => !e);

            if (seagulls.Count >= maintainCount)
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

            var instance = Instantiate(prefab, position, Quaternion.Euler(0.0f, Random.value * 360.0f, 0.0f));
            seagulls.Add(instance);
        }
    }
}
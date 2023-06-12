using UnityEngine;

namespace ShootingRangeGame.Scripts.Pickups
{
    [DisallowMultipleComponent]
    public sealed class SlingshotDrawLine : MonoBehaviour
    {
        [SerializeField] private Transform mid;
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 twistOffset;

        private float volume;
        private Slingshot slingshot;

        private void Awake()
        {
            slingshot = GetComponentInParent<Slingshot>();
        }

        private void Update()
        {
            if (!slingshot) return;
            
            var start = transform.position;
            var end = target.position;
            var distance = (end - start).magnitude;
            var stretch = slingshot.SlackDistance / Mathf.Max(distance, slingshot.SlackDistance);

            mid.position = (start + end) * 0.5f;
            mid.localScale = new Vector3(1.0f, stretch, stretch);
            
            mid.rotation = Quaternion.LookRotation(start - end, slingshot.transform.up) * Quaternion.Euler(twistOffset);
        }
    }
}
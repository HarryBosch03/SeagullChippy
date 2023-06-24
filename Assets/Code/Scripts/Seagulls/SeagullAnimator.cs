using UnityEngine;

namespace ShootingRangeGame.Seagulls
{
    [DisallowMultipleComponent]
    public sealed class SeagullAnimator : MonoBehaviour
    {
        [SerializeField] private float stepDistance;
        [SerializeField] private Transform[] feetVisualContainers;
        [SerializeField] private float smoothing;
        [SerializeField] private float overstep;

        [Space]
        [SerializeField] private float idleDelay = 0.2f;
        [SerializeField] private float idleOffset = 0.1f;

        private Seagull seagull;
        private new Rigidbody rigidbody;
        private float distanceCounter;
        private float idleTimer;

        private Foot[] feet = new Foot[2];
        private int freeFoot;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            seagull = GetComponent<Seagull>();
            for (var i = 0; i < feet.Length; i++)
            {
                feet[i] = new Foot(transform, feetVisualContainers[i]);
            }
        }

        private void FixedUpdate()
        {
            if (!seagull.Grounded) return;
            UpdateCounter();
            UpdateFootPositions();
        }

        private void LateUpdate()
        {
            if (seagull.Grounded)
            {
                MoveFeet();
            }
            else
            {
                for (var i = 0; i < feet.Length && i < feetVisualContainers.Length; i++)
                {
                    var foot = feet[i];
                    var visuals = feetVisualContainers[i];

                    foot.cPosition = visuals.position - visuals.rotation * foot.offset;
                    foot.cRotation = visuals.rotation;

                    foot.tPosition = foot.cPosition;
                    foot.tRotation = foot.cRotation;
                }
            }
        }

        private void UpdateCounter()
        {
            var velocity = rigidbody.velocity;
            var speed = Mathf.Sqrt(velocity.x * velocity.x + velocity.z * velocity.z);
            distanceCounter += speed * Time.deltaTime;

            var idle = speed < 0.01f;
            if (idle) idleTimer += Time.deltaTime;
            else idleTimer = 0.0f;
        }

        private void UpdateFootPositions()
        {
            if (idleTimer > idleDelay)
            {
                for (var i = 0; i < feet.Length; i++)
                {
                    if (idleTimer > idleDelay + idleOffset * i)
                    {
                        feet[(freeFoot + i) % feet.Length].tPosition = transform.position;
                    }   
                }
            }
            else if (distanceCounter > stepDistance)
            {
                var direction = rigidbody.velocity.normalized;
                feet[freeFoot].tPosition = transform.position + direction * (stepDistance * (1.0f + overstep));
                feet[freeFoot].tRotation = transform.rotation;

                freeFoot = (freeFoot + 1) % feet.Length;
                distanceCounter -= stepDistance;
            }

            foreach (var foot in feet)
            {
                var t = smoothing >= Time.deltaTime ? Time.deltaTime / smoothing : 1.0f;
                foot.cPosition = Vector3.Lerp(foot.cPosition, foot.tPosition, t);
                foot.cRotation = Quaternion.Slerp(foot.cRotation, foot.tRotation, t);
            }
        }

        private void MoveFeet()
        {
            for (var i = 0; i < feet.Length && i < feetVisualContainers.Length; i++)
            {
                var foot = feet[i];
                var visuals = feetVisualContainers[i];

                visuals.position = foot.cPosition + foot.cRotation * foot.offset;
                visuals.rotation = foot.cRotation;
            }
        }

        class Foot
        {
            public Vector3 tPosition, cPosition;
            public Quaternion tRotation, cRotation;
            public Vector3 offset;

            public Foot(Transform transform, Transform visualContainer)
            {
                tPosition = cPosition = transform.position;
                tRotation = cRotation = transform.rotation;
                offset = transform.InverseTransformPoint(visualContainer.position);
            }
        }
    }
}
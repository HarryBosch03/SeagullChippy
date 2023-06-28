using UnityEngine;

namespace ShootingRangeGame.Seagulls
{
    [DisallowMultipleComponent]
    public sealed class BirdAnimator : MonoBehaviour
    {
        [SerializeField] private float stepDistance;
        [SerializeField] private Transform[] feetVisualContainers;
        [SerializeField] private float smoothing;
        [SerializeField] private float overstep;

        [Space]
        [SerializeField] private float idleDelay = 0.2f;
        [SerializeField] private float idleOffset = 0.1f;

        private Bird bird;
        private new Rigidbody rigidbody;
        private float distanceCounter;
        private float idleTimer;

        private Foot[] feet = new Foot[2];
        private int freeFoot;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            bird = GetComponent<Bird>();
            for (var i = 0; i < feet.Length; i++)
            {
                feet[i] = new Foot(transform, feetVisualContainers[i]);
            }
        }

        private void FixedUpdate()
        {
            if (!bird.Grounded) return;
            UpdateCounter();
            UpdateFootPositions();
        }

        private void LateUpdate()
        {
            if (bird.Grounded)
            {
                MoveFeet();
            }
            else
            {
                for (var i = 0; i < feet.Length && i < feetVisualContainers.Length; i++)
                {
                    var foot = feet[i];
                    var visuals = feetVisualContainers[i];

                    foot.currentPosition = visuals.position - visuals.rotation * foot.translationOffset;
                    foot.currentRotation = visuals.rotation;

                    foot.targetPosition = foot.currentPosition;
                    foot.targetRotation = foot.currentRotation;
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
                        feet[(freeFoot + i) % feet.Length].targetPosition = transform.position;
                    }   
                }
            }
            else if (distanceCounter > stepDistance)
            {
                var direction = rigidbody.velocity.normalized;
                feet[freeFoot].targetPosition = transform.position + direction * (stepDistance * (1.0f + overstep));
                feet[freeFoot].targetRotation = transform.rotation;

                freeFoot = (freeFoot + 1) % feet.Length;
                distanceCounter -= stepDistance;
            }

            foreach (var foot in feet)
            {
                var t = smoothing >= Time.deltaTime ? Time.deltaTime / smoothing : 1.0f;
                foot.currentPosition = Vector3.Lerp(foot.currentPosition, foot.targetPosition, t);
                foot.currentRotation = Quaternion.Slerp(foot.currentRotation, foot.targetRotation, t);
            }
        }

        private void MoveFeet()
        {
            for (var i = 0; i < feet.Length && i < feetVisualContainers.Length; i++)
            {
                var foot = feet[i];
                var footBone = feetVisualContainers[i];

                footBone.position = foot.currentPosition + foot.currentRotation * foot.translationOffset;
                //footBone.rotation = foot.currentRotation * foot.rotationOffset;
            }
        }

        class Foot
        {
            public Vector3 targetPosition, currentPosition;
            public Quaternion targetRotation, currentRotation;
            public Vector3 translationOffset;
            public Quaternion rotationOffset;

            public Foot(Transform transform, Transform animationRoot)
            {
                targetPosition = currentPosition = transform.position;
                targetRotation = currentRotation = transform.rotation;
                translationOffset = transform.InverseTransformPoint(animationRoot.position);
                rotationOffset = Quaternion.Inverse(animationRoot.rotation) * transform.rotation;
            }
        }
    }
}
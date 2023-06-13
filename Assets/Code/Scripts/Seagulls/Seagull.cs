using System;
using ShootingRangeGame.AI.BehaviourTrees.Core;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace ShootingRangeGame.Seagulls
{
    public class Seagull : MonoBehaviour, IHasBehaviourTree
    {
        [Header("AI")] 
        [SerializeField] private SeagullBrain brain;

        [Header("MOVEMENT")] [Space] 
        [SerializeField] private float moveSpeed;
        [SerializeField] private float floatiness = 4.0f;

        [SerializeField] private float accelerationTime;
        [SerializeField] private float rotationSmoothing;

        [Header("DEATH")] [Space]
        [SerializeField] private float minDamageForce = 15.0f;
        [SerializeField] private float seagullExplosionForce = 20.0f;
        [SerializeField] [Range(0.0f, 1.0f)] private float seagullExplosionRandomness = 0.5f;
        [SerializeField] private bool invulnerable;
        [SerializeField] private float lastCollisionForce;
        [SerializeField] private float lastValidCollisionForce;
        [SerializeField] private Transform detachOnDeath;
        [SerializeField] private GameObject bloodTrail;

        public ParticleSystem particles;

        public new Rigidbody rigidbody;

        public Vector3 MoveVector { get; set; }
        public Vector3 LookDirection { get; set; }
        public MonoBehaviour Behaviour => this;
        public BehaviourTree Tree => brain.Tree;
        public bool Grounded { get; private set; }

        public static event Action OnSeagullHit;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();

            brain.Init(this);
            LookDirection = Quaternion.Euler(0.0f, Random.value * 360.0f, 0.0f) * Vector3.forward;
        }

        private void Update()
        {
            brain.Update();
        }

        private void FixedUpdate()
        {
            Move();
            SetGroundedState();
        }

        private void Move()
        {
            var target = MoveVector * moveSpeed;
            var difference = (target - rigidbody.velocity);
            difference.y = 0.0f;
            var force = Vector3.ClampMagnitude(difference, moveSpeed) / accelerationTime;
            rigidbody.AddForce(force, ForceMode.Acceleration);

            if (LookDirection.magnitude > 0.1f)
            {
                var tRotation = Quaternion.LookRotation(LookDirection, Vector3.up);
                rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, tRotation, Time.deltaTime / rotationSmoothing);
            }
            else
            {
                rigidbody.rotation = Quaternion.identity;
            }

            rigidbody.angularVelocity = Vector3.zero;
            
            rigidbody.AddForce(Vector3.up * -rigidbody.velocity.y * floatiness, ForceMode.Acceleration);
        }

        private void SetGroundedState()
        {
            Grounded = false;
            var queries = Physics.OverlapSphere(transform.position, 0.1f);
            foreach (var query in queries)
            {
                if (query.transform.IsChildOf(transform)) continue;
                Grounded = true;
                break;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            var collisionForce = collision.relativeVelocity.magnitude;
            lastCollisionForce = collisionForce;

            if (collisionForce > minDamageForce)
            {
                lastValidCollisionForce = collisionForce;
                Hit(collision.relativeVelocity);
            }
        }

        public void Hit(Vector3 force)
        {
            OnSeagullHit?.Invoke();
            particles.Play();

            Die(force);
        }

        private void Die(Vector3 force)
        {
            if (invulnerable) return;

            if (detachOnDeath) detachOnDeath.SetParent(null);

            var children = GetComponentsInChildren<MeshRenderer>();
            foreach (var child in children)
            {
                var rb = child.gameObject.AddComponent<Rigidbody>();
                rb.velocity = Vector3.Lerp(force, Random.insideUnitSphere.normalized, seagullExplosionRandomness) * seagullExplosionForce;
                rb.transform.SetParent(null);

                child.gameObject.AddComponent<BoxCollider>();

                if (bloodTrail)
                {
                    var btInstance = Instantiate(bloodTrail, child.transform);
                    btInstance.transform.localPosition = Vector3.zero;
                    btInstance.transform.localRotation = Quaternion.identity;
                }
            }

            Destroy(gameObject);
        }
    }
}
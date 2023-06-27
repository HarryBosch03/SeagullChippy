using ShootingRangeGame.AI.BehaviourTrees.Core;
using ShootingRangeGame.VFX;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShootingRangeGame.Seagulls
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public class Bird : MonoBehaviour, IHasBehaviourTree
    {
        [Header("AI")] 
        [SerializeField] private BirdBrain brain;
        [SerializeField] private BirdType birdType;

        [Header("MOVEMENT")] 
        [Space] 
        [SerializeField] private float moveSpeed;

        [SerializeField] private float floatiness = 4.0f;

        [SerializeField] private float accelerationTime;
        [SerializeField] private float rotationSmoothing;

        [Header("DEATH")] [Space] 
        [SerializeField] private float minDamageForce = 15.0f;

        [SerializeField] private bool invulnerable;
        [SerializeField] private FXGroup hitFX;

        [Space] 
        [SerializeField] private ParticleSystem wetFX;

        public new Rigidbody rigidbody;

        private static Transform container;

        public Vector3 MoveVector { get; set; }
        public Vector3 LookDirection { get; set; }
        public MonoBehaviour Behaviour => this;
        public BehaviourTree Tree => brain.Tree;
        public bool Grounded { get; private set; }
        public float Wet { get; set; }
        public BirdType Type => birdType;

        private static readonly string[] Names = 
        {
            "Goose", "Duck", "Swan", "Pigeon", "Chicken", "Frog", "Rat", "Horse"
        };

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();

            brain.Init(this);
            LookDirection = Quaternion.Euler(0.0f, Random.value * 360.0f, 0.0f) * Vector3.forward;

            gameObject.name = Names[Random.Range(0, Names.Length)];

            if (!container)
            {
                container = new GameObject("--- Seagulls ---").transform;
                container.transform.position = Vector3.zero;
                container.transform.rotation = Quaternion.identity;
                container.transform.localScale = Vector3.one;
            }
            transform.SetParent(container);
            transform.localScale = Vector3.one * Random.Range(0.8f, 1.2f);
        }

        private void Update()
        {
            brain.Update();

            Wet -= Time.deltaTime;
            var isWet = Wet > 0.0f;
            
            //if (isWet && !wetFX.isEmitting) wetFX.Play();
            //if (!isWet && wetFX.isEmitting) wetFX.Stop();

            Wet = Mathf.Max(Wet, 0.0f);
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
            if (collisionForce > minDamageForce)
            {
                Die();
            }
        }


        [ContextMenu("Kill All")]
        public void KillAll()
        {
            var seagulls = FindObjectsOfType<Bird>();
            foreach (var seagull in seagulls)
            {
                seagull.Die();
            }
        }
        
        [ContextMenu("Kill")]
        public void Die()
        {
            if (invulnerable) return;

            FXGroup.Try(hitFX, fx => fx.Instance().At(this).Play().AndDestroy());

            Destroy(gameObject);
        }

        public enum BirdType
        {
            Seagull,
            Pigeon,
        }
    }
}
using ShootingRangeGame.AI.BehaviourTrees.Core;
using ShootingRangeGame.AI.BehaviourTrees.Leaves;
using ShootingRangeGame.Seagulls.Leaves;
using UnityEngine;

namespace ShootingRangeGame.Seagulls
{
    [System.Serializable]
    public class SeagullBrain : IHasBehaviourTree
    {
        [SerializeField] private string defaultAnimation;
        [SerializeField] private float maxWanderDistance;

        private DirectionPreprocess directionPreprocess = new();

        public Seagull Seagull { get; private set; }

        public BehaviourTree Tree { get; private set; } = new(
            new SelectorLeaf()
                // .AddChild(new SequenceLeaf()
                //     .AddChild(new Startled())
                //     .AddChild(new Fly()))
                .AddChild(new CheckForWater())
                .AddChild(new EatFood())
                .AddChild(new RandomLeaf()
                    .AddChild(new Wait(), 1.5f)
                    .AddChild(new Wander(), 1.0f)
                    .AddChild(new Fly(), 0.15f)
                ));

        private Animator animator;

        public string Animation { get; set; }

        public void Init(Seagull seagull)
        {
            this.Seagull = seagull;

            animator = seagull.GetComponent<Animator>();
            Animation = defaultAnimation;

            Tree.Init(this);
        }

        public void Update()
        {
            Seagull.MoveVector = Vector3.zero;
            Seagull.LookDirection = directionPreprocess.Apply(Seagull.transform, Seagull.LookDirection);
            Tree.Execute(this);

            animator.Play(Animation);
            Animation = defaultAnimation;
        }

        public void ShiftLookDirection(float variance)
        {
            var transform = Seagull.transform;

            var distance = Mathf.Sqrt(transform.position.x * transform.position.x + transform.position.z * transform.position.z);
            if (distance > maxWanderDistance) Seagull.LookDirection = -new Vector3(transform.position.x, 0.0f, transform.position.z).normalized;
            Seagull.LookDirection = Quaternion.Euler(0.0f, Random.value * 2.0f * variance - variance, 0.0f) * Seagull.LookDirection;
        }

        public MonoBehaviour Behaviour => Seagull;

        public void MoveTowards(Vector3 point, float speed = 1.0f)
        {
            var direction = point - Seagull.transform.position;
            direction.y = 0.0f;
            Seagull.MoveVector = direction.normalized * speed;
            Seagull.LookDirection = direction.normalized;
        }
    }
}
﻿using ShootingRangeGame.AI.BehaviourTrees.Core;
using ShootingRangeGame.AI.BehaviourTrees.Leaves;
using UnityEngine;

namespace ShootingRangeGame.Seagulls
{
    [System.Serializable]
    public partial class SeagullBrain : IHasBehaviourTree
    {
        [SerializeField] private string defaultAnimation;
        [SerializeField] private float maxWanderDistance;

        public Seagull Seagull { get; private set; }

        public BehaviourTree Tree { get; private set; } = new(
            new SequenceLeaf()
                .AddChild(new DirectionPreprocess())
                .AddChild(
                    new RandomLeaf()
                        .AddChild(new Wait(), 1.5f)
                        .AddChild(new Wander(), 1.0f)
                        .AddChild(new Fly(), 0.15f)
                )
        );

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
    }
}
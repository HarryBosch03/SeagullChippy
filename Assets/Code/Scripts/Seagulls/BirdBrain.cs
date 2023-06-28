using System;
using System.Collections.Generic;
using ShootingRangeGame.AI.BehaviourTrees.Core;
using ShootingRangeGame.AI.BehaviourTrees.Leaves;
using ShootingRangeGame.Seagulls.Leaves;
using Unity.Burst;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShootingRangeGame.Seagulls
{
    [System.Serializable]
    public class BirdBrain : IHasBehaviourTree
    {
        public const int AIUpdateFrequency = 5;
        
        [SerializeField] private float maxWanderDistance;

        private DirectionPreprocess directionPreprocess = new();

        public Bird Bird { get; private set; }
        public BehaviourTree Tree { get; private set; }
        
        private int updateIndex;
        private static int nextUpdateIndex;
        
        public static event Action<BirdBrain> EatEvent;

        public void Init(Bird bird)
        {
            Bird = bird;
            updateIndex = nextUpdateIndex;
            updateIndex = (updateIndex + 1) % AIUpdateFrequency;
            
            Tree = new BehaviourTree(
                new SelectorLeaf()
                    // .AddChild(new SequenceLeaf()
                    //     .AddChild(new Startled())
                    //     .AddChild(new Fly()))
                    .AddChild(new CheckForWater())
                    .AddChild(new EatFood().SetCallback(EatCallback))
                    .AddChild(new RandomLeaf()
                        .AddChild(new Wait(), 1.5f)
                        .AddChild(new Wander(), 1.0f)
                        .AddChild(new Fly(), 0.15f)
                    ));

            Tree.Init(this);
        }

        public void FixedUpdate()
        {
            if (Time.frameCount % updateIndex != updateIndex) return;

            Bird.MoveVector = Vector3.zero;
            Bird.LookDirection = directionPreprocess.Apply(Bird.transform, Bird.LookDirection);
            Tree.Execute(this);

            Bird.Animation = Bird.DefaultAnimation;
        }

        public void ShiftLookDirection(float variance)
        {
            var transform = Bird.transform;

            var distance = Mathf.Sqrt(transform.position.x * transform.position.x + transform.position.z * transform.position.z);
            if (distance > maxWanderDistance) Bird.LookDirection = -new Vector3(transform.position.x, 0.0f, transform.position.z).normalized;
            Bird.LookDirection = Quaternion.Euler(0.0f, Random.value * 2.0f * variance - variance, 0.0f) * Bird.LookDirection;
        }

        public MonoBehaviour Behaviour => Bird;

        public void MoveTowards(Vector3 point, float speed = 1.0f)
        {
            var direction = point - Bird.transform.position;
            direction.y = 0.0f;
            Bird.MoveVector = direction.normalized * speed;
            Bird.LookDirection = direction.normalized;
        }

        private void EatCallback() => EatEvent?.Invoke(this);
    }
}
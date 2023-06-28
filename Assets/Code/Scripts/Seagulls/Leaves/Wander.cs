using ShootingRangeGame.AI.BehaviourTrees.Core;
using UnityEngine;

namespace ShootingRangeGame.Seagulls.Leaves
{
    public class Wander : Leaf<BirdBrain>
    {
        private float wanderTime;
        private float timer;

        public override string Name => $"{base.Name} Wander";

        protected override void OnStart(BehaviourTree tree)
        {
            wanderTime = Random.value * 0.2f + 0.6f;
            timer = 0.0f;

            Target.ShiftLookDirection(10.0f);
        }

        protected override BehaviourTree.Result OnExecute(BehaviourTree tree)
        {
            if (timer > wanderTime) return BehaviourTree.Result.Success;

            Target.Bird.MoveVector = Target.Bird.LookDirection;

            timer += Time.deltaTime;
            return BehaviourTree.Result.Pending;
        }
    }
}
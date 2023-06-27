using ShootingRangeGame.AI.BehaviourTrees.Core;
using UnityEngine;

namespace ShootingRangeGame.Seagulls.Leaves
{
    public class CheckForWater : Leaf<BirdBrain>
    {
        public const float WaterLevel = -0.6f;

        private float timer;

        public override string Name => $"{base.Name} Check For Water";
        public override BehaviourTree.AbandonResponse RespondToAbandonRequest() => BehaviourTree.AbandonResponse.CannotAbandon;

        protected override BehaviourTree.Result OnExecute(BehaviourTree tree)
        {
            var seagull = Target.Bird;
            if (timer > 0.0f)
            {
                seagull.MoveVector = Vector3.back * 3.0f;
                seagull.LookDirection = Vector3.back;

                timer -= Time.deltaTime;
                return timer > 0.0f ? BehaviourTree.Result.Pending : BehaviourTree.Result.Success;
            }

            if (seagull.transform.position.y < WaterLevel - 0.0001f)
            {
                timer = 1.0f + Random.value;
                seagull.MoveVector = Vector3.back * 3.0f;
                seagull.LookDirection = Vector3.back;
                seagull.Wet = 1.0f + Random.value * 2.0f;
                return BehaviourTree.Result.Pending;
            }

            return BehaviourTree.Result.Failure;
        }
    }
}
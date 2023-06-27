using ShootingRangeGame.AI.BehaviourTrees.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShootingRangeGame.Seagulls.Leaves
{
    public class Fly : Leaf<BirdBrain>
    {
        private float launchForce = 2.0f;
        private float minFlyForce = 10.0f;
        private float maxFlyForce = 16.0f;
        private float flySpeed = 2.0f;

        private float targetHeight;
        private int state;

        public override string Name => $"{base.Name} Fly";

        protected override void OnStart(BehaviourTree tree)
        {
            Target.ShiftLookDirection(10.0f);
            targetHeight = Target.Bird.transform.position.y + Random.value * 2.0f + 1.0f;
            Target.Bird.rigidbody.AddForce(Vector3.up * launchForce, ForceMode.VelocityChange);
            state = 0;
        }

        public override BehaviourTree.AbandonResponse RespondToAbandonRequest() => BehaviourTree.AbandonResponse.CannotAbandon;

        protected override BehaviourTree.Result OnExecute(BehaviourTree tree)
        {
            var transform = Target.Bird.transform;

            Target.Bird.MoveVector = Target.Bird.LookDirection * flySpeed;

            switch (state)
            {
                case 0:
                    var height = transform.position.y;
                    if (height > targetHeight)
                    {
                        state++;
                        break;
                    }

                    Target.Animation = "Flap";
                    var flyForce = Mathf.Lerp(maxFlyForce, minFlyForce, height / targetHeight);
                    Target.Bird.rigidbody.AddForce(Vector3.up * flyForce, ForceMode.Acceleration);
                    break;
                case 1:
                    Target.Animation = "Glide";
                    if (Target.Bird.Grounded) state++;
                    break;
                default:
                    return BehaviourTree.Result.Success;
            }

            return BehaviourTree.Result.Pending;
        }
    }
}
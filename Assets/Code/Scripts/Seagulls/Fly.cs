using ShootingRangeGame.AI.BehaviourTrees.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShootingRangeGame.Seagulls
{
    public partial class SeagullBrain
    {
        public class Fly : Leaf<SeagullBrain>
        {
            private float launchForce = 8.0f;
            private float flyForce = 8.0f;
            private float flySpeed = 2.0f;
            
            private float targetHeight;
            private int state;
            
            public override string Name => $"{base.Name} Fly";
            public override bool CanReturnPending => true;

            protected override void OnStart(BehaviourTree tree)
            {
                Target.ShiftLookDirection(10.0f);
                targetHeight = Target.Seagull.transform.position.y + Random.value * 4.0f + 1.0f;
                state = 0;
            }

            protected override BehaviourTree.Result OnExecute(BehaviourTree tree)
            {
                var transform = Target.Seagull.transform;
                
                Target.Seagull.MoveVector = Target.Seagull.LookDirection * flySpeed;

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
                        Target.Seagull.rigidbody.AddForce(Vector3.up * flyForce, ForceMode.Acceleration);
                        break;
                    case 1:
                        Target.Animation = "Glide";
                        if (Target.Seagull.Grounded) state++;
                        break;
                    default:
                        return BehaviourTree.Result.Success;
                }
                
                return BehaviourTree.Result.Pending;
            }
        }
    }
}
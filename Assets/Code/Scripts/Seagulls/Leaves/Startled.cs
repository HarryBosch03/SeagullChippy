using ShootingRangeGame.AI.BehaviourTrees.Core;
using UnityEngine;

namespace ShootingRangeGame.Seagulls.Leaves
{
    public class Startled : Leaf<BirdBrain>
    {
        public float startleDistance = 5.0f;
        public float threshold = 8.0f;

        protected override BehaviourTree.Result OnExecute(BehaviourTree tree)
        {
            var seagull = Target.Bird;
            var list = Physics.OverlapSphere(seagull.transform.position, startleDistance);
            foreach (var element in list)
            {
                if (!element.attachedRigidbody) continue;
                if (element.attachedRigidbody.velocity.magnitude < threshold) continue;
                
                return BehaviourTree.Result.Success;
            }
            
            return BehaviourTree.Result.Failure;
        }
    }
}
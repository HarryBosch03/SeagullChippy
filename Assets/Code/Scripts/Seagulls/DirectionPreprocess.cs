using System;
using ShootingRangeGame.AI.BehaviourTrees.Core;
using UnityEngine;

namespace ShootingRangeGame.Seagulls
{
    public class DirectionPreprocess : Leaf<SeagullBrain>
    {
        public float correctionScale = 0.1f;
        public float correctionDeadzone = 0.1f;
        private float lookaheadDistance = 5.0f;
        
        protected override BehaviourTree.Result OnExecute(BehaviourTree tree)
        {
            var transform = Target.Seagull.transform;
            var vector = -transform.position;
            vector.y = 0.0f;

            var distance = vector.magnitude;
            var direction = vector / distance;

            var angle = Vector3.SignedAngle(direction, Target.Seagull.LookDirection, Vector3.up);
            var delta = angle * Mathf.Max(distance - correctionDeadzone, 0.0f) * correctionScale * Time.deltaTime;
            
            Target.Seagull.LookDirection = Quaternion.Euler(Vector3.up * delta) * Target.Seagull.LookDirection;

            Target.Seagull.LookDirection = Lookahead();

            return BehaviourTree.Result.Success;
        }

        public Vector3 Lookahead()
        {
            var transform = Target.Seagull.transform;
            var direction = Target.Seagull.LookDirection;
            
            var ray = new Ray(transform.position, direction);
            if (Physics.Raycast(ray, out var hit, lookaheadDistance))
            {
                var rays = new[]
                {
                    new Ray(transform.position, Quaternion.Euler(Vector3.up * -45.0f) * direction),
                    new Ray(transform.position, Quaternion.Euler(Vector3.up * 45.0f) * direction),
                };

                var hits = new RaycastHit[rays.Length];
                for (var i = 0; i < rays.Length; i++)
                {
                    ray = rays[i];
                    if (!Physics.Raycast(ray, out hit, lookaheadDistance)) return ray.direction;
                    hits[i] = hit;
                }

                var best = 0;
                for (var i = 1; i < hits.Length; i++)
                {
                    var scoreBest = hits[best].distance;
                    var scoreCurrent = hits[i].distance;

                    if (scoreCurrent > scoreBest) best = i;
                }
                return rays[best].direction;
            }
            
            return direction; 
        }
    }
}
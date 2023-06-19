using UnityEngine;

namespace ShootingRangeGame.Seagulls
{
    public class DirectionPreprocess
    {
        public float correctionScale = 0.1f;
        public float correctionDeadzone = 12.0f;
        private float lookaheadDistance = 5.0f;
        private float minDistance = 4.0f;

        public Vector3 Apply(Transform transform, Vector3 lookDirection)
        {
            var vector = -transform.position;
            vector.y = 0.0f;

            var distance = vector.magnitude;
            var direction = vector / distance;
            if (distance >= correctionDeadzone)
            {
                var angle = -Vector3.SignedAngle(direction, lookDirection, Vector3.up);
                var delta = angle * Mathf.Max(distance - correctionDeadzone, 0.0f) * correctionScale * Time.deltaTime;

                lookDirection = Quaternion.Euler(Vector3.up * delta) * lookDirection;
            }

            if (distance < minDistance)
            {
                lookDirection = transform.position;
                lookDirection.y = 0.0f;
                lookDirection.Normalize();
            }
            
            lookDirection = Lookahead(transform, lookDirection);
            return lookDirection;
        }

        public Vector3 Lookahead(Transform transform, Vector3 direction)
        {
            bool raycast(Ray ray, out RaycastHit hit)
            {
                var queries = Physics.RaycastAll(ray, lookaheadDistance);
                hit = default;
                var found = false;
                foreach (var query in queries)
                {
                    if (query.collider.transform.IsChildOf(transform)) continue;
                    if (!found)
                    {
                        found = true;
                        hit = query;
                        continue;
                    }

                    if (query.distance < hit.distance) hit = query;
                }
                return found;
            }
            
            var ray = new Ray(transform.position, direction);
            if (raycast(ray, out var hit))
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
                    if (!raycast(ray, out hit)) return ray.direction;
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
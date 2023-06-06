using System;
using System.Collections;
using System.IO;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace ShootingRangeGame.Seagulls
{
    [System.Serializable]
    public class SeagullBrain
    {
        [SerializeReference] private float minWanderTime;
        [SerializeReference] private float maxWanderTime;
        [SerializeReference] private float wanderDistance;

        private Seagull seagull;

        private Vector3 wanderDirection;
        private float speed;

        private State state;
        private float changeStateTime;
        private float stateTimer;

        public void Init(Seagull seagull)
        {
            this.seagull = seagull;

            TransitionToRandomState();
            wanderDirection = Random.insideUnitSphere;
            wanderDirection.y = 0.0f;
            wanderDirection.Normalize();
        }

        public void Update()
        {
            seagull.MoveDirection = Vector3.zero;

            stateTimer += Time.deltaTime;
            if (stateTimer > changeStateTime)
            {
                TransitionToRandomState();
            }

            switch (state)
            {
                case State.Wander:
                    seagull.MoveDirection = wanderDirection.normalized * speed;
                    seagull.LookDirection = seagull.MoveDirection;
                    break;
                case State.Wait:
                    seagull.LookDirection = wanderDirection.normalized;
                    break;
                case State.Count:
                default:
                    TransitionToRandomState();
                    break;
            }
        }

        public void TransitionToRandomState()
        {
            var newState = (State)Random.Range(0, (int)State.Count);
            ChangeState(newState);
        }

        public void ChangeState(State state)
        {
            this.state = state;
            changeStateTime = Random.Range(minWanderTime, maxWanderTime);
            speed = Mathf.Lerp(0.5f, 1.0f, Random.value);
            stateTimer = 0.0f;

            GetNewDirection();
        }

        private void GetNewDirection(int i = 0)
        {
            if (i > 50) return;

            var angle = (10.0f + Random.value * 20.0f) * (Random.value > 0.5f ? 1.0f : -1.0f);
            
            wanderDirection = (Quaternion.Euler(0.0f, angle, 0.0f) * wanderDirection).normalized;
            var ray = new Ray(seagull.transform.position, wanderDirection);
            if (!Physics.Raycast(ray)) return;
            GetNewDirection(i + 1);
        }

        public enum State
        {
            Wander,
            Wait,
            Count,
        }
    }
}
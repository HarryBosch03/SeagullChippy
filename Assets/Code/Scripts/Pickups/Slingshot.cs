using System;
using System.Collections;
using HandyVR.Bindables.Pickups;
using HandyVR.Interfaces;
using HandyVR.Player;
using HandyVR.Player.Hands;
using UnityEngine;

namespace ShootingRangeGame.Pickups
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(VRPickup))]
    public sealed class Slingshot : MonoBehaviour
    {
        [SerializeField] private SlingyThing slingyThing;
        [SerializeField] private float slackDistance = 0.1f;
        [SerializeField] private float slingStrength = 10.0f;
        [SerializeField] private float spring = 100.0f;
        [SerializeField] private float damping = 10.0f;
        [SerializeField] private float rumbleStrength = 0.1f;
        [SerializeField] private float tension;
        [SerializeField] private float drawingHandBindingRange = 0.2f;

        private VRPickup pickup;
        private Transform slingyThingTarget;

        public float SlackDistance => slackDistance;
        public Vector3 Target => slingyThingTarget.position;
        public Vector3 Current => slingyThing.position;
        public VRHand DrawingHand { get; private set; }
        public Vector3 Tension { get; private set; }

        public float TensionMagnitude
        {
            get => tension;
            private set => tension = value;
        }

        public bool Tense => TensionMagnitude > 0.0f;

        private void Awake()
        {
            slingyThingTarget = new GameObject($"{slingyThing.transform.name} Target").transform;
            slingyThingTarget.SetParent(slingyThing.transform.parent);
            slingyThingTarget.localPosition = slingyThing.transform.localPosition;
            slingyThingTarget.localRotation = slingyThing.transform.localRotation;

            slingyThing.position = slingyThing.transform.position;

            pickup = GetComponent<VRPickup>();
        }

        private void Update()
        {
            LookForDrawingHand();

            UpdateTension();
            slingyThing.Update();
            UpdateControllerRumble();
        }

        private void LookForDrawingHand()
        {
            if (!pickup.ActiveBinding) return;
            if (DrawingHand)
            {
                if (!DrawingHand.Input.Trigger.Down) ReleaseSlingshot();
                return;
            }

            var hands = FindObjectsOfType<VRHand>();
            foreach (var hand in hands)
            {
                if (hand.BindingController == pickup.ActiveBinding.target) continue;
                if ((hand.transform.position - transform.position).magnitude > drawingHandBindingRange) continue;
                if (!hand.Input.Trigger.Down) continue;

                DrawingHand = hand;
                break;
            }
        }

        private void ReleaseSlingshot()
        {
            if (DrawingHand.ActiveBinding)
            {
                var ammo = DrawingHand.ActiveBinding.bindable;
                if (ammo.Rigidbody)
                {
                    ammo.ActiveBinding.Deactivate();
                    ammo.Rigidbody.AddForce(Tension * slingStrength, ForceMode.Impulse);
                    StartCoroutine(IgnoreCollisionWith(ammo));
                }
            }

            DrawingHand = null;
        }

        private IEnumerator IgnoreCollisionWith(IVRBindable ammo)
        {
            HandyVR.Utility.Physics.IgnoreCollision(ammo.gameObject, gameObject, true);
            yield return new WaitForSeconds(0.08f);
            HandyVR.Utility.Physics.IgnoreCollision(ammo.gameObject, gameObject, false);
        }

        private void UpdateControllerRumble()
        {
            if (!DrawingHand) return;
            if (!pickup.ActiveBinding) return;
            if (pickup.ActiveBinding.target is not VRHandBinding hand) return;
            hand.Hand.Input.Rumble(TensionMagnitude * rumbleStrength, Time.deltaTime);
        }

        private void FixedUpdate()
        {
            UpdateTension();
            UpdateSlingyThing();
            slingyThing.FixedUpdate();
        }

        private void UpdateTension()
        {
            var difference = slingyThingTarget.position - slingyThing.position;
            var distance = difference.magnitude;
            TensionMagnitude = (distance - slackDistance) * spring;
            Tension = Tense ? difference.normalized * TensionMagnitude : Vector3.zero;
        }

        private void UpdateSlingyThing()
        {
            Vector3 force;
            if (DrawingHand)
            {
                force = -slingyThing.velocity / Time.deltaTime;
                slingyThing.position = DrawingHand.transform.position;
            }
            else
            {
                force = Physics.gravity;
                if (Tense)
                {
                    force += Tension - slingyThing.velocity * damping;
                }
            }

            slingyThing.acceleration += force;
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            if (!slingyThingTarget) return;

            Gizmos.color = Color.Lerp(Color.green, Color.red, tension).Normalized();
            Gizmos.DrawLine(slingyThing.position, slingyThingTarget.position);
        }

        [Serializable]
        public class SlingyThing
        {
            public Transform transform;

            [HideInInspector] public Vector3 position;
            [HideInInspector] public Vector3 velocity;
            [HideInInspector] public Vector3 acceleration;

            public void FixedUpdate()
            {
                position += velocity * Time.deltaTime;
                velocity += acceleration * Time.deltaTime;
                acceleration = Vector3.zero;
            }

            public void Update()
            {
                transform.position = position;
            }
        }
    }
}
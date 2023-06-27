using HandyVR.Bindables;
using HandyVR.Bindables.Pickups;
using HandyVR.Interfaces;
using ShootingRangeGame.Audio;
using UnityEngine;

namespace ShootingRangeGame.Pickups
{
    [RequireComponent(typeof(Slingshot))]
    public class SlingshotFX : MonoBehaviour
    {
        [SerializeField] private AudioClipGroup dryFireClip;
        [SerializeField] private AudioClipGroup fireClip;
        [SerializeField] private AudioClipGroup pickupClip;
        [SerializeField] private AudioClipGroup dropClip;

        private AudioSource source;
        private Slingshot slingshot;
        private VRPickup pickup;

        private void Awake()
        {
            slingshot = GetComponent<Slingshot>();
            pickup = GetComponent<VRPickup>();
            source = GetComponentInChildren<AudioSource>();
        }

        private void OnEnable()
        {
            pickup.BindEvent += OnBind;
            pickup.UnbindEvent += OnUnbind;

            slingshot.FireEvent += OnFire;
        }

        private void OnDisable()
        {
            pickup.BindEvent -= OnBind;
            pickup.UnbindEvent -= OnUnbind;

            slingshot.FireEvent -= OnFire;
        }

        private void OnBind(VRBinding newBinding)
        {
            if (pickupClip) pickupClip.Play(source, transform.position);
        }

        private void OnUnbind(VRBinding oldBinding)
        {
            if (dropClip) dropClip.Play(source, transform.position);
        }

        private void OnFire(IVRBindable bindable)
        {
            var clip = IVRBindable.Valid(bindable) ? fireClip : dryFireClip;
            if (clip) clip.Play(source, transform.position);
        }
    }
}
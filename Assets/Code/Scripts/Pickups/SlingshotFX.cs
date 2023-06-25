using HandyVR.Bindables;
using HandyVR.Bindables.Pickups;
using HandyVR.Interfaces;
using UnityEngine;

namespace ShootingRangeGame.Pickups
{
    [RequireComponent(typeof(Slingshot))]
    public class SlingshotFX : MonoBehaviour
    {
        [SerializeField] private AudioClip dryFireClip;
        [SerializeField] private AudioClip fireClip;
        [SerializeField] private AudioClip pickupClip;
        [SerializeField] private AudioClip dropClip;

        private Slingshot slingshot;
        private VRPickup pickup;

        private void Awake()
        {
            slingshot = GetComponent<Slingshot>();
            pickup = GetComponent<VRPickup>();
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
            AudioSource.PlayClipAtPoint(pickupClip, transform.position);
        }

        private void OnUnbind(VRBinding oldBinding)
        {
            AudioSource.PlayClipAtPoint(dropClip, transform.position);
        }

        private void OnFire(IVRBindable bindable)
        {
            AudioSource.PlayClipAtPoint(IVRBindable.Valid(bindable) ? fireClip : dryFireClip, transform.position);
        }
    }
}
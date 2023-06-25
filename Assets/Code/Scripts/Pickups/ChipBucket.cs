using System;
using HandyVR.Bindables;
using HandyVR.Bindables.Pickups;
using HandyVR.Interfaces;
using HandyVR.Player;
using HandyVR.Player.Input;
using UnityEngine;

namespace ShootingRangeGame.Pickups
{
    public class ChipBucket : MonoBehaviour, IVRHandle
    {
        [SerializeField] private VRPickup chipPrefab;

        private new Rigidbody rigidbody;
        
        public VRBinding ActiveBinding => null;
        public Rigidbody Rigidbody => rigidbody;
        public event Action<VRBinding> BindEvent;
        public event Action<VRBinding> UnbindEvent;

        private void Awake()
        {
            rigidbody = gameObject.GetOrAddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
        }

        private void OnEnable()
        {
            IVRBindable.All.Add(this);
        }

        private void OnDisable()
        {
            IVRBindable.All.Remove(this);
        }

        public void OnBindingActivated(VRBinding newBinding)
        {
            newBinding.Deactivate();
            BindEvent?.Invoke(null);
        }

        public void OnBindingDeactivated(VRBinding oldBinding)
        {
            var instance = Instantiate(chipPrefab, oldBinding.target.BindingPosition, oldBinding.target.BindingRotation);
            new VRBinding(instance, oldBinding.target);
            UnbindEvent?.Invoke(null);
        }

        public void InputCallback(VRHand hand, IVRBindable.InputType type, HandInput.InputWrapper input) { }

        public bool IsValid() => true;
    }
}

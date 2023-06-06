using HandyVR.Bindables;
using HandyVR.Bindables.Pickups;
using HandyVR.Interfaces;
using HandyVR.Player;
using HandyVR.Player.Input;
using UnityEngine;

namespace ShootingRangeGame
{
    public class ChipBucket : MonoBehaviour, IVRHandle
    {
        [SerializeField] private VRPickup chipPrefab;

        private new Rigidbody rigidbody;
        
        public VRBinding ActiveBinding => null;
        public Rigidbody Rigidbody => rigidbody;

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
        }

        public void OnBindingDeactivated(VRBinding oldBinding)
        {
            var instance = Instantiate(chipPrefab, transform.position, transform.rotation);
            new VRBinding(instance, oldBinding.target);
        }

        public void InputCallback(VRHand hand, IVRBindable.InputType type, HandInput.InputWrapper input) { }

        public bool IsValid() => true;
    }
}

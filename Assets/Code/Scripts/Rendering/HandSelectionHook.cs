using HandyVR.Player.Hands;
using UnityEngine;

namespace ShootingRangeGame.Rendering
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(VRHandBinding))]
    public class HandSelectionHook : MonoBehaviour
    {
        private VRHandBinding binding;

        private void Awake()
        {
            binding = GetComponent<VRHandBinding>();
        }

        private void Update()
        {
            if (binding.PointingAt != null)
            {
                SelectionRenderFeature.Add(binding.PointingAt.gameObject);
            }
        }
    }
}
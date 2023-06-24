using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine;

namespace ShootingRangeGame
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public sealed class ShadowCamera : MonoBehaviour
    {
        [SerializeField] private int smoothing;

        private Camera self;
        private Camera target;

        private Vector3 position;
        private Quaternion rotation;

        private void Awake()
        {
            self = GetComponent<Camera>();
            target = FindTarget();
        }

        private void FixedUpdate()
        {
            var normalizedDelta = smoothing > Time.deltaTime ? 1.0f / smoothing : 1.0f;
            position = Vector3.Lerp(position, target.transform.position, normalizedDelta);
            rotation = Quaternion.Slerp(rotation, target.transform.rotation, normalizedDelta);
        }

        private void Update()
        {
            self.transform.position = position;
            self.transform.rotation = rotation;

            self.fieldOfView = target.fieldOfView;
            self.nearClipPlane = target.nearClipPlane;
            self.farClipPlane = target.farClipPlane;

            self.stereoTargetEye = StereoTargetEyeMask.None;
        }

        private static Camera FindTarget()
        {
            var cameraOffset = FindObjectOfType<CameraOffset>();
            return cameraOffset ? cameraOffset.gameObject.GetComponentInChildren<Camera>() : null;
        }
    }
}
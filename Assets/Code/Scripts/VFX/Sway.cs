using UnityEngine;
using Random = UnityEngine.Random;

namespace ShootingRangeGame.VFX
{
    
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class Sway : MonoBehaviour
    {
        [SerializeField] private SwayProfile profile;
        [SerializeField] private float weight = 1.0f;

        private Quaternion rotation;
        private float offset;

        private void Start()
        {
            rotation = transform.localRotation;
            offset = Random.value;
        }

        private void Update()
        {
            var wind = profile.GetRotation(offset, weight);
            
            transform.rotation = wind * transform.parent.rotation * rotation;
        }
    }
}

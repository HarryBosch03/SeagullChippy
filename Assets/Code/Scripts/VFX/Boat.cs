using UnityEngine;
using Random = UnityEngine.Random;

namespace ShootingRangeGame.VFX
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public class Boat : MonoBehaviour
    {
        [SerializeField] private Vector3 translation = new Vector3(0.0f, 0.2f, 0.0f);
        [SerializeField] private Vector3 rotation = new Vector3(3.0f, 0.0f, 0.0f);
        [SerializeField] private float frequency = 0.6f;
        [SerializeField] private float variance = 1.0f;

        private float t;
        private Vector3 translationOffset;
        private Quaternion rotationOffset;

        private void Awake()
        {
            translationOffset = transform.position;
            rotationOffset = transform.rotation;

            t = Random.value * variance;
        }

        private void Update()
        {
            var sin = Mathf.Sin(t);
            var cos = Mathf.Cos(t);
            
            transform.position = translationOffset + translation * sin;
            transform.rotation = rotationOffset * Quaternion.Euler(rotation * cos);
            
            t += Time.deltaTime * frequency;
        }
    }
}

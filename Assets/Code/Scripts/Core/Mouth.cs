using System.Collections;
using System.Collections.Generic;
using HandyVR.Bindables;
using HandyVR.Bindables.Pickups;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShootingRangeGame
{
    public class Mouth : MonoBehaviour
    {
        [SerializeField] private float range;
        [SerializeField] private List<VRBindingType> food;
        [SerializeField] private GameObject eatFX;
        [SerializeField] private float eatFXLifetime;
        [SerializeField] private AudioClip[] clips;

        private void FixedUpdate()
        {
            var queries = Physics.OverlapSphere(transform.position, range);
            foreach (var query in queries)
            {
                var pickup = query.transform.GetComponentInParent<VRPickup>();
                if (!pickup) continue;
                if (!pickup.BindingType) continue;
                if (!food.Contains(pickup.BindingType)) continue;

                if (eatFX)
                {
                    var instance = Instantiate(eatFX, transform.position, transform.rotation);
                    Destroy(instance, eatFXLifetime);
                }

                StartCoroutine(PlaySounds());
                Destroy(pickup.gameObject);
            }
        }

        private IEnumerator PlaySounds()
        {
            for (var i = 0; i < 3; i++)
            {
                AudioSource.PlayClipAtPoint(clips[Random.Range(0, clips.Length)], transform.position);
                yield return new WaitForSeconds(0.2f);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.2f);
            Gizmos.DrawSphere(transform.position, range);
        }
    }
}

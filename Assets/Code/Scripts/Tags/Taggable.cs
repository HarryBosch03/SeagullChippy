using System.Collections.Generic;
using UnityEngine;

namespace ShootingRangeGame.Tags
{
    public class Taggable : MonoBehaviour
    {
        [SerializeField] private List<Tag> tags;

        public List<Tag> Tags => tags;

        public bool HasTag(Tag tag)
        {
            foreach (var other in tags)
            {
                if (Tag.Same(other, tag)) return true;
            }
            return false;
        }

        public static bool HasTag(Component target, Tag tag)
        {
            if (!tag) return true;
            if (!target) return false;
            var taggable = target as Taggable ?? target.GetComponent<Taggable>();

            return taggable && taggable.HasTag(tag);
        }
    }
}
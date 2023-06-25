using UnityEngine;

namespace ShootingRangeGame.Tags
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Tag")]
    public class Tag : ScriptableObject
    {
        public static Tag Make(string name)
        {
            var instance = CreateInstance<Tag>();
            instance.name = name;
            return instance;
        }

        public static void Cache(ref Tag tag, string name)
        {
            if (tag) return;
            tag = Make(name);
        }
        
        public static bool Same(Tag a, Tag b)
        {
            if (a == b) return true;
            if (!a || !b) return false;

            return a.name == b.name;
        }
    }
}
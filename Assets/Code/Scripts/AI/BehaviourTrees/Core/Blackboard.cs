using System.Collections.Generic;

namespace ShootingRangeGame.AI.BehaviourTrees.Core
{
    public class Blackboard
    {
        private Dictionary<string, object> data = new();

        public bool Has(string key) => data.ContainsKey(key);
        
        public void Set(string key, object value)
        {
            if (data.ContainsKey(key)) data[key] = value;
            else data.Add(key, value);
        }

        public T Get<T>(string key, T fallback = default)
        {
            if (!data.ContainsKey(key)) data.Add(key, fallback);
            return (T)data[key];
        }
    }
}
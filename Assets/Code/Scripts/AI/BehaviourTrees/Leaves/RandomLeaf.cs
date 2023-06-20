using System;
using System.Collections.Generic;
using ShootingRangeGame.AI.BehaviourTrees.Core;
using Random = UnityEngine.Random;

namespace ShootingRangeGame.AI.BehaviourTrees.Leaves
{
    public class RandomLeaf : CompositeLeaf
    {
        public readonly Dictionary<Leaf, float> weights = new();
        public float defaultWeight = 1.0f;

        public Leaf pendingChild;

        public override string Name => $"{base.Name} Random Leaf";
        public float GetWeight(Leaf leaf) => weights.ContainsKey(leaf) ? weights[leaf] : defaultWeight;

        public void SetWeight(Leaf leaf, float weight)
        {
            if (weights.ContainsKey(leaf)) weights[leaf] = weight;
            else weights.Add(leaf, weight);
        }

        public RandomLeaf AddChild(Leaf leaf, float weight)
        {
            SetWeight(leaf, weight);
            base.AddChild(leaf);
            return this;
        }

        public CompositeLeaf SetDefaultWeight(float defaultWeight)
        {
            this.defaultWeight = defaultWeight;
            return this;
        }

        public Leaf GetRandomChild(List<Leaf> pool)
        {
            var totalWeight = 0.0f;
            foreach (var child in pool)
            {
                totalWeight += weights[child];
            }

            var weight = Random.value * totalWeight;
            foreach (var child in pool)
            {
                var other = weights[child];
                if (weight < other) return child;
                weight -= other;
            }

            return pool[^1];
        }

        protected override BehaviourTree.Result OnExecute(BehaviourTree tree)
        {
            var child = pendingChild;
            pendingChild = null;

            var pool = new List<Leaf>(children);
            while (pool.Count > 0)
            {
                if (child != null)
                {
                    switch (child.Execute())
                    {
                        case BehaviourTree.Result.Success:
                            return BehaviourTree.Result.Success;
                        case BehaviourTree.Result.Failure:
                            break;
                        case BehaviourTree.Result.Pending:
                            pendingChild = child;
                            return BehaviourTree.Result.Pending;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    pool.Remove(child);
                }
                child = GetRandomChild(pool);
            }

            return BehaviourTree.Result.Failure;
        }
    }
}
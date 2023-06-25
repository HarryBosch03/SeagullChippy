using System.Collections.Generic;
using ShootingRangeGame.AI.BehaviourTrees.Core;
using UnityEngine;

namespace ShootingRangeGame.AI.BehaviourTrees.Leaves
{
    public class RandomLeaf : CompositeLeaf
    {
        public const int Shuffles = 100;

        public readonly Dictionary<Leaf, float> weights = new();
        public float defaultWeight = 1.0f;

        private List<Leaf> pool;

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

        protected override BehaviourTree.Result OnExecute(BehaviourTree tree)
        {
            if (LastEvaluationResult != BehaviourTree.Result.Pending)
            {
                pool = GetPool();
                pool = ShufflePool(pool);
            }

            return SimpleLoop(
                res => res == BehaviourTree.AbandonResponse.WithSuccess,
                BehaviourTree.Result.Success,
                BehaviourTree.Result.Success,
                BehaviourTree.Result.Failure,
                pool);
        }

        private List<Leaf> GetPool()
        {
            var pool = new List<Leaf>();
            if (pendingChildIndex != -1)
            {
                pool.Add(children[pendingChildIndex]);
            }

            for (var i = 0; i < children.Count; i++)
            {
                if (i == pendingChildIndex) continue;
                var child = children[i];
                pool.Add(child);
            }

            pendingChildIndex = -1;
            return pool;
        }

        private List<Leaf> ShufflePool(List<Leaf> input)
        {
            var output = new List<Leaf>();

            while (input.Count > 0)
            {
                var r = random();
                input.Remove(r);
                output.Add(r);
            }

            return output;

            Leaf random()
            {
                var totalWeight = 0.0f;
                foreach (var i in input)
                {
                    totalWeight += weights[i];
                }

                var weight = Random.value * totalWeight;
                foreach (var i in input)
                {
                    if (weight < weights[i]) return i;
                    weight -= weights[i];
                }

                return input[^1];
            }
        }
    }
}
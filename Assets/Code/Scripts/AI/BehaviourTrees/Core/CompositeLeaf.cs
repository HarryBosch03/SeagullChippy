using System;
using System.Collections.Generic;

namespace ShootingRangeGame.AI.BehaviourTrees.Core
{
    public abstract class CompositeLeaf : Leaf
    {
        public readonly List<Leaf> children = new();
        protected int pendingChildIndex = -1;
        private bool initialized;

        public override string Name => $"{base.Name}[COMPOSITE]";
        public Leaf PendingChild => children.Iwf(pendingChildIndex);

        public override void Init(BehaviourTree tree) => RecursiveAction(tree, ctx =>
        {
            if (ctx.leaf is CompositeLeaf compositeLeaf)
            {
                if (compositeLeaf.initialized) return;
                compositeLeaf.initialized = true;
            }

            ctx.leaf.Init(tree);
        });

        public CompositeLeaf AddChild(params Leaf[] leaf)
        {
            children.AddRange(leaf);
            return this;
        }

        protected int GetStartingIndex()
        {
            var i = pendingChildIndex == -1 ? 0 : pendingChildIndex;
            pendingChildIndex = -1;
            return i;
        }

        public bool CheckForAbandonment(out BehaviourTree.AbandonResponse response)
        {
            response = BehaviourTree.AbandonResponse.WithSuccess;

            if (pendingChildIndex == -1) return false;

            foreach (var child in children)
            {
                if (!child.OverridePending()) continue;
                response = child.RespondToAbandonRequest();
                switch (response)
                {
                    default:
                    case BehaviourTree.AbandonResponse.WithSuccess:
                    case BehaviourTree.AbandonResponse.WithFailure:
                        pendingChildIndex = -1;
                        break;
                    case BehaviourTree.AbandonResponse.CannotAbandon:
                        break;
                }
                return true;
            }

            return false;
        }

        protected BehaviourTree.Result SimpleLoop(
            Func<BehaviourTree.AbandonResponse, bool> abandonmentCallback,
            BehaviourTree.Result abandonmentReturn,
            BehaviourTree.Result compare,
            BehaviourTree.Result fallback) => SimpleLoop(abandonmentCallback, abandonmentReturn, compare, fallback, children);

        protected BehaviourTree.Result SimpleLoop(
            Func<BehaviourTree.AbandonResponse, bool> abandonmentCallback,
            BehaviourTree.Result abandonmentReturn,
            BehaviourTree.Result compare,
            BehaviourTree.Result fallback,
            IList<Leaf> children)
        {
            if (CheckForAbandonment(out var abandonResponse))
            {
                if (abandonmentCallback(abandonResponse))
                {
                    return abandonmentReturn;
                }
            }

            for (var i = GetStartingIndex(); i < children.Count; i++)
            {
                var child = children[i];
                var res = child.Execute();
                if (res == BehaviourTree.Result.Pending)
                {
                    pendingChildIndex = i;
                    return BehaviourTree.Result.Pending;
                }

                if (res == compare) return compare;
            }

            return fallback;
        }

        public override LeafContext RecursiveAction(LeafContext parent, Action<LeafContext> callback)
        {
            var context = base.RecursiveAction(parent, callback);
            foreach (var child in children)
            {
                child.RecursiveAction(context, callback);
            }

            return context;
        }

        public override BehaviourTree.AbandonResponse RespondToAbandonRequest() => PendingChild?.RespondToAbandonRequest() ?? BehaviourTree.AbandonResponse.WithSuccess;
    }
}
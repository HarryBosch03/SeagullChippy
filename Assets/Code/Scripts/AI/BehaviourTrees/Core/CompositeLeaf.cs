using System;
using System.Collections.Generic;
using UnityEditor.Hardware;
using UnityEditor.Search;

namespace ShootingRangeGame.AI.BehaviourTrees.Core
{
    public abstract class CompositeLeaf : Leaf
    {
        public readonly List<Leaf> children = new();
        private int pendingChild;
        private bool initialized;

        public override string Name => $"{base.Name}[COMPOSITE]";

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

        protected BehaviourTree.Result SimpleLoop(BehaviourTree.Result compare, BehaviourTree.Result fallback)
        {
            var i = pendingChild;
            pendingChild = 0;

            for (; i < children.Count; i++)
            {
                var child = children[i];
                var res = child.Execute();
                if (res == BehaviourTree.Result.Pending)
                {
                    pendingChild = i;
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
    }
}
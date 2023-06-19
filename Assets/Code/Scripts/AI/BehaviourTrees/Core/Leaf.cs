using System;

namespace ShootingRangeGame.AI.BehaviourTrees.Core
{
    public abstract class Leaf
    {
        public virtual string Name => $"[{(Tree != null ? Tree.Target.GetType().Name : "UNINITIALIZED")}]";

        public BehaviourTree Tree { get; private set; }
        public BehaviourTree.Result LastEvaluationResult { get; private set; }

        public virtual bool CanReturnPending => false;
        
        public virtual void Init(BehaviourTree tree)
        {
            this.Tree = tree;
        }

        public BehaviourTree.Result Execute()
        {
            if (CanReturnPending && LastEvaluationResult != BehaviourTree.Result.Pending)
            {
                OnStart(Tree);
            }
            
            var res = OnExecute(Tree);
            if (res != BehaviourTree.Result.Pending && LastEvaluationResult == BehaviourTree.Result.Pending)
            {
                OnCleanup(Tree);
            }

            LastEvaluationResult = res;
            return res;
        }

        public LeafContext RecursiveAction(BehaviourTree tree, Action<LeafContext> callback) => RecursiveAction(new LeafContext(tree, this, 0, null), callback);
        public virtual LeafContext RecursiveAction(LeafContext parent, Action<LeafContext> callback)
        {
            var context = new LeafContext(parent.tree, this, parent.depth + 1, parent.leaf); 
            callback(context);
            return context;
        }
        
        protected abstract BehaviourTree.Result OnExecute(BehaviourTree tree);
        protected virtual void OnStart(BehaviourTree tree) { }
        protected virtual void OnCleanup(BehaviourTree tree) { }
    }

    public abstract class Leaf<T> : Leaf
    {
        public T Target => (T)Tree.Target;
    }
}
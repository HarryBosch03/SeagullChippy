using ShootingRangeGame.AI.BehaviourTrees.Core;

namespace ShootingRangeGame.AI.BehaviourTrees.Leaves
{
    public class DynamicLeaf : Leaf
    {
        public readonly string subName;
        public readonly ExecutionCallback executionCallback;
        public readonly AbandonmentCallback abandonmentCallback;

        public DynamicLeaf(string subName, ExecutionCallback executionCallback, AbandonmentCallback abandonmentCallback)
        {
            this.subName = subName;
            this.executionCallback = executionCallback;
            this.abandonmentCallback = abandonmentCallback;
        }

        public override string Name => $"{base.Name}[DYNAMIC] {subName}";
        public override BehaviourTree.AbandonResponse RespondToAbandonRequest() => abandonmentCallback();
        protected override BehaviourTree.Result OnExecute(BehaviourTree tree) => executionCallback();
        
        public delegate BehaviourTree.Result ExecutionCallback();
        public delegate BehaviourTree.AbandonResponse AbandonmentCallback();
    }

    public sealed class DynamicLeaf<T> : DynamicLeaf 
    {
        public DynamicLeaf(string subName, ExecutionCallback executionCallback, AbandonmentCallback abandonmentCallback) : base(subName, executionCallback, abandonmentCallback) { }
        public T Target => (T)Tree.Target;
    }
}

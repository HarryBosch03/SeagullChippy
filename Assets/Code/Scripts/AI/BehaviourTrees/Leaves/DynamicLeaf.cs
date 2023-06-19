using ShootingRangeGame.AI.BehaviourTrees.Core;

namespace ShootingRangeGame.AI.BehaviourTrees.Leaves
{
    public class DynamicLeaf : Leaf
    {
        public readonly string subName;
        public readonly Callback callback;

        public DynamicLeaf(string subName, Callback callback)
        {
            this.subName = subName;
            this.callback = callback;
        }

        public override string Name => $"{base.Name}[DYNAMIC] {subName}";
        protected override BehaviourTree.Result OnExecute(BehaviourTree tree) => callback();
        
        public delegate BehaviourTree.Result Callback();
    }

    public sealed class DynamicLeaf<T> : DynamicLeaf 
    {
        public DynamicLeaf(string subName, Callback callback) : base(subName, callback) { }

        public T Target => (T)Tree.Target;
    }
}

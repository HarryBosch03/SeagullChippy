namespace ShootingRangeGame.AI.BehaviourTrees.Core
{
    public sealed class BehaviourTree
    {
        public readonly Leaf root;
        
        public object Target { get; private set; }
        public BehaviourTree() { }
        
        public BehaviourTree(Leaf root)
        {
            this.root = root;
        }

        public void Init(object target)
        {
            Target = target;
            root.Init(this);
        }

        public void Execute(object target)
        {
            Target = target;
            root.Execute();
        }

        public enum Result
        {
            Success,
            Failure,
            Pending,
        }
    }
}
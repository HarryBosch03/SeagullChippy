namespace ShootingRangeGame.AI.BehaviourTrees.Core
{
    public abstract class DecoratorLeaf : Leaf
    {
        private Leaf child;
        
        public override string Name => $"{base.Name}[DECORATOR]";
        
        public DecoratorLeaf SetChild(Leaf leaf)
        {
            child = leaf;
            return this;
        }
    }
}
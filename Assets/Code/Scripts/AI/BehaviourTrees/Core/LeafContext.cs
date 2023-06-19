namespace ShootingRangeGame.AI.BehaviourTrees.Core
{
    public class LeafContext
    {
        public BehaviourTree tree;
        public Leaf leaf;
        public int depth;
        public Leaf parent;

        public LeafContext(BehaviourTree tree, Leaf leaf, int depth, Leaf parent)
        {
            this.leaf = leaf;
            this.depth = depth;
            this.parent = parent;
        }
    }
}
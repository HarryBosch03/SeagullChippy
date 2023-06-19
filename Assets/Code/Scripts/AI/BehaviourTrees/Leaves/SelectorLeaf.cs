using ShootingRangeGame.AI.BehaviourTrees.Core;

namespace ShootingRangeGame.AI.BehaviourTrees.Leaves
{
    public sealed class SelectorLeaf : CompositeLeaf
    {
        public override string Name => $"{base.Name} Selector Leaf";
        protected override BehaviourTree.Result OnExecute(BehaviourTree tree) => SimpleLoop(BehaviourTree.Result.Success, BehaviourTree.Result.Failure);
    }
}
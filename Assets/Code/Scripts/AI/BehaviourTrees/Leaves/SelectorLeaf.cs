using System;
using ShootingRangeGame.AI.BehaviourTrees.Core;

namespace ShootingRangeGame.AI.BehaviourTrees.Leaves
{
    public sealed class SelectorLeaf : CompositeLeaf
    {
        public override string Name => $"{base.Name} Selector Leaf";
        protected override BehaviourTree.Result OnExecute(BehaviourTree tree) => SimpleLoop(
            res => res == BehaviourTree.AbandonResponse.WithSuccess, 
            BehaviourTree.Result.Success,
            BehaviourTree.Result.Success, 
            BehaviourTree.Result.Failure);
    }
}
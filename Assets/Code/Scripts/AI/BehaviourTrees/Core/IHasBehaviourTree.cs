using ShootingRangeGame.Core;

namespace ShootingRangeGame.AI.BehaviourTrees.Core
{
    public interface IHasBehaviourTree : IHasMonoBehaviour
    {
        BehaviourTree Tree { get; }
    }
}
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace ShootingRangeGame.Core
{
    public interface IHasMonoBehaviour
    {
        MonoBehaviour Behaviour { get; }
    }
}
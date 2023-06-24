using System;

namespace ShootingRangeGame.Saves
{
    [Serializable]
    public class SaveData : ICloneable
    {
        public int highScore;
        
        public object Clone() => MemberwiseClone();
    }
}
using System;

namespace ShootingRangeGame.Saves
{
    [System.Serializable]
    public class SaveData : ICloneable
    {
        public int highScore;
        
        public object Clone() => this.MemberwiseClone();
    }
}
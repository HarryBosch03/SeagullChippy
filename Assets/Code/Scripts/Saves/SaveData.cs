using System;

namespace ShootingRangeGame.Saves
{
    [Serializable]
    public class SaveData : ICloneable
    {
        public int highScore;

        public float masterVolume = 1.0f;
        public float effectVolume = 1.0f;
        public float soundtrackVolume = 1.0f;

        public object Clone() => MemberwiseClone();

        public override string ToString()
        {
            return $"Save Data:\n" +
                   $"\tHigh Score: {highScore}\n" +
                   $"\tMaster Volume: {masterVolume}\n" +
                   $"\tEffect Volume: {effectVolume}\n" +
                   $"\tSoundtrack Volume: {soundtrackVolume}";
        }
    }
}
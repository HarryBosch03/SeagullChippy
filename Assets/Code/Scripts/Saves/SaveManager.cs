using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Rendering.UI;
using Debug = UnityEngine.Debug;

namespace ShootingRangeGame.Saves
{
    public static class SaveManager
    {
        public static int Slot { get; set; } = 0;
        public static SaveData SaveData { get; private set; }

        public static string SaveLocation => $"{Application.persistentDataPath}Saves/Slot.{Slot + 1}";

        public static SaveData GetOrLoad()
        {
            if (SaveData == null) Load();
            return SaveData;
        }

        public static void Load()
        {
            if (!Directory.Exists(Path.GetDirectoryName(SaveLocation)) || !File.Exists(SaveLocation))
            {
                SaveData = new SaveData();
                Debug.Log($"Created Save Slot {Slot + 1}");
                Debug.Log(SaveData);
                return;
            }

            using var fs = new FileStream(SaveLocation, FileMode.Open);
            var bf = new BinaryFormatter();
            SaveData = (SaveData)bf.Deserialize(fs);
            Debug.Log($"Loaded Save Slot {Slot + 1}");
            Debug.Log(SaveData);
        }

        public static void Save()
        {
            var directory = Path.GetDirectoryName(SaveLocation);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var fs = new FileStream(SaveLocation, FileMode.OpenOrCreate);
            var bf = new BinaryFormatter();
            bf.Serialize(fs, SaveData);
            
            Debug.Log($"Saved data to Slot {Slot + 1}");
        }
    }
}
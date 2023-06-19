using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;

namespace ShootingRangeGame.Saves
{
    public static class SaveManager
    {
        public static int Slot { get; set; } = 0;
        public static SaveData SaveData { get; private set; }

        private static SaveData saveCache;
        private static string saveLocationCache;
        private static Thread saveThread;

        public static SaveResult LastSaveResult { get; private set; }
        public static bool SaveInProgress => saveThread != null && saveThread.IsAlive;
        public static string SaveLocation => $"{Application.persistentDataPath}Saves/Slot.{Slot + 1}";
        public static event System.Action SaveStartEvent_MainThread; 
        public static event System.Action SaveStartEvent_SaveThread; 
        public static event System.Action SaveEndEvent_SaveThread; 

        public static SaveData GetOrLoad()
        {
            if (SaveData == null) Load();
            return SaveData;
        }
        
        public static void Load()
        {
            if (!File.Exists(SaveLocation))
            {
                SaveData = new SaveData();
                return;
            }
            
            using var fs = new FileStream(SaveLocation, FileMode.Open);
            var bf = new BinaryFormatter();
            SaveData = (SaveData)bf.Deserialize(fs);
        }

        public static void Save()
        {
            if (SaveInProgress)
            {
                saveThread.Join();
            }

            saveCache = (SaveData)SaveData.Clone();
            saveLocationCache = SaveLocation;
            saveThread = new Thread(SaveOnThread);
            saveThread.Start();
            
            SaveStartEvent_MainThread?.Invoke();
        }

        public static void SaveOnThread()
        {
            LastSaveResult = new SaveResult(LastSaveResult);

            try
            {
                var timer = new Stopwatch();
                timer.Start();

                SaveStartEvent_SaveThread?.Invoke();

                using var fs = new FileStream(SaveLocation, FileMode.OpenOrCreate);
                var bf = new BinaryFormatter();
                bf.Serialize(fs, saveCache);

                SaveEndEvent_SaveThread?.Invoke();

                timer.Stop();

                LastSaveResult.successful = true;
                LastSaveResult.saveTime = timer.Elapsed;
            }
            catch (System.Exception ex)
            {
                LastSaveResult.exception = ex;
            }
        }

        public class SaveResult
        {
            public int id = 0;
            public bool successful = false;
            public TimeSpan saveTime = TimeSpan.Zero;
            public System.Exception exception = null;

            public SaveResult(SaveResult previous)
            {
                if (previous == null) return;
                id = previous.id + 1;
            }
        }
    }
}
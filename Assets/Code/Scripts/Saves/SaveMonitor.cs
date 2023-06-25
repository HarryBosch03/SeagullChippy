using UnityEngine;

namespace ShootingRangeGame.Saves
{
    public class SaveMonitor : MonoBehaviour
    {
        private int lastSaveID;

        private void Update()
        {
            if (SaveManager.SaveInProgress) return;

            if (SaveManager.LastSaveResult == null) return;

            var results = SaveManager.LastSaveResult;
            if (results.id == lastSaveID) return;

            lastSaveID = results.id;
            if (results.successful)
            {
                Debug.Log($"Save finished Successfully in {results.saveTime:[-]'ss[.FFFFFFF]}ms");
            }
            else
            {
                Debug.LogError($"Save Failed!\n{results.exception}");
            }
        }
    }
}
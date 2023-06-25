using System;
using System.Collections;
using ShootingRangeGame.Session;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShootingRangeGame.UI
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class MenuActions : MonoBehaviour
    {
        public static event Action<float> CountdownEvent;
        
        public void StartSession()
        {
            var session = FindObjectOfType<GameSession>();
            StartCoroutine(StartRoutine(session));
        }

        private IEnumerator StartRoutine(GameSession session)
        {
            var timer = 3.5f;
            while (timer > 0.0f)
            {
                CountdownEvent(timer);
                timer -= Time.deltaTime;
                yield return null;
            }

            session.StartRound();
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
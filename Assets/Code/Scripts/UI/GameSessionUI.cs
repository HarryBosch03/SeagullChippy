using System;
using TMPro;
using UnityEngine;

namespace ShootingRangeGame.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class GameSessionUI : MonoBehaviour
    {
        [SerializeField] private Driver driver;
        [SerializeField] private string fallbackText = string.Empty;

        private TMP_Text text;
        [SerializeField] private string template;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
            template = text.text;
        }

        private void Update()
        {
            text.text = string.Format(template, Get());
        }

        public object Get()
        {
            var session = GameSession.Active;
            if (!session) return fallbackText;
            
            return driver switch
            {
                Driver.Score => session.Score,
                Driver.HighScore => session.HighScore,
                Driver.Timer => TimeSpan.FromSeconds(session.RoundTimer),
                _ => null
            };
        }

        public enum Driver
        {
            Score,
            HighScore,
            Timer,
        }
    }
}
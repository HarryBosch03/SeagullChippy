using System;
using ShootingRangeGame.Audio;
using ShootingRangeGame.Saves;
using ShootingRangeGame.Seagulls;
using UnityEngine;

namespace ShootingRangeGame.Session
{
    public class GameSession : MonoBehaviour
    {
        [Header("Round Settings")]
        [SerializeField] private float roundLength;
        [SerializeField] private bool autoStartRoundOnAwake;
        [SerializeField] private bool endless;
        [SerializeField] private int feedSeagullPoints = 1;
        [SerializeField] private int feedPigeonPoints = -2;

        [Header("Audio")]
        [SerializeField] private AudioClipGroup roundStartAudio;
        [SerializeField] private AudioClipGroup roundEndAudio;

        private bool roundActive;
        
        public int Score { get; private set; }
        public float RoundTimer { get; private set; }
        public static GameSession Active { get; private set; }
        public static int HighScore
        {
            get => SaveManager.GetOrLoad().highScore;
            set => SaveManager.GetOrLoad().highScore = value;
        }

        public static event Action StartSessionEvent;
        public static event Action EndSessionEvent;
        
        private void Start()
        {
            if (autoStartRoundOnAwake)
            {
                StartRound();
            }
        }

        private void Update()
        {
            if (roundActive)
            {
                if (!endless) RoundTimer -= Time.deltaTime;
                if (RoundTimer <= 0.0f) EndRound();
            }
        }

        [ContextMenu("Start Round")]
        public void StartRound()
        {
            if (Active) Active.EndRound();
            
            Active = this;
            RoundTimer = roundLength + 2.0f;
            roundActive = true;
            Score = 0;
            roundStartAudio.Play();
            StartSessionEvent?.Invoke();

            BirdBrain.EatEvent += OnEat;
        }

        private void OnEat(BirdBrain bird)
        {
            switch (bird.Bird.Type)
            {
                case Bird.BirdType.Seagull:
                    AwardPoint(feedSeagullPoints);
                    break;
                case Bird.BirdType.Pigeon:
                    AwardPoint(feedPigeonPoints);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [ContextMenu("End Round")]
        public void EndRound()
        {
            if (!roundActive) return;
            
            roundActive = false;
            RoundTimer = 0;
            roundEndAudio.Play();

            if (Score > HighScore) HighScore = Score;
            SaveManager.Save();
            EndSessionEvent?.Invoke();

            BirdBrain.EatEvent -= OnEat;
        }

        public static void AwardPoint(int increment = 1)
        {
            if (!Active) return;
            Active.Score += increment;
        }

        public static void SetScore(int score)
        {
            if (!Active) return;
            Active.Score = score;
        }

        public static void ResetScore() => SetScore(0);
    }
}

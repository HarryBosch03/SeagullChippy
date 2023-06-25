using System;
using ShootingRangeGame.AI.BehaviourTrees.Core;
using ShootingRangeGame.Saves;
using ShootingRangeGame.Seagulls;
using TMPro;
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
        [SerializeField] private AudioSource roundStart;
        [SerializeField] private AudioSource roundEnd;

        private int highScore;
        private bool roundActive;
        
        public int Score { get; private set; }
        public float RoundTimer { get; private set; }
        public static GameSession Active { get; private set; }
        public static int HighScore
        {
            get => SaveManager.GetOrLoad().highScore;
            set => SaveManager.GetOrLoad().highScore = value;
        }
        
        public void OnAwake()
        {
            roundStart = GetComponent<AudioSource>();
            roundEnd = GetComponent<AudioSource>();
        }
        
        private void Start()
        {
            highScore = HighScore;
            
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
            if (roundStart) roundStart.Play();

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
            Active = null;
            
            roundActive = false;
            RoundTimer = 0;
            if (roundEnd) roundEnd.Play();

            if (Score > HighScore) HighScore = Score;
            SaveManager.Save();
            
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

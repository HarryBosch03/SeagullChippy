using ShootingRangeGame.AI.BehaviourTrees.Core;
using ShootingRangeGame.Saves;
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

        [Header("Components")]
        [SerializeField] private TextMeshProUGUI timerReadout;
        [SerializeField] private TextMeshProUGUI scoreReadout;
        [SerializeField] private TextMeshProUGUI highScoreReadout;

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
            RefreshUI();
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

            RefreshUI();
        }

        [ContextMenu("Start Round")]
        public void StartRound()
        {
            if (Active) Active.EndRound();
            
            Active = this;
            RoundTimer = roundLength;
            roundActive = true;
            Score = 0;
            if (roundStart) roundStart.Play();
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
        }

        public void RefreshUI()
        {
            if (roundActive)
            {
                if (timerReadout) timerReadout.text = (int)(RoundTimer / 60) + ":" + (int)(RoundTimer % 60);
            }

            if (scoreReadout) scoreReadout.text = Score.ToString();
            if (highScoreReadout) highScoreReadout.text = highScore.ToString();
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

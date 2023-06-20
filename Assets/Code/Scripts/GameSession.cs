using ShootingRangeGame.Saves;
using ShootingRangeGame.Seagulls;
using System;
using TMPro;
using UnityEngine;

namespace ShootingRangeGame
{
    public class GameSession : MonoBehaviour
    {
        [Header("Round Settings")]
        public float roundLength;

        private int highScore;
        private bool roundActive = false;

        [Header("Components")]
        public TextMeshProUGUI timerReadout;
        public TextMeshProUGUI scoreReadout;
        public TextMeshProUGUI highScoreReadout;

        [SerializeField] private AudioSource roundStart;
        [SerializeField] private AudioSource roundEnd;
        

        public void OnAwake()
        {
            roundStart = GetComponent<AudioSource>();
            roundEnd = GetComponent<AudioSource>();
        }

        public int Score { get; private set; }
        public int HighScore
        {
            get => SaveManager.GetOrLoad().highScore;
            set => SaveManager.GetOrLoad().highScore = value;
        }
        
        public float RoundTimer { get; private set; }
        public static GameSession Active { get; private set; }

        private void OnEnable()
        {
            Seagull.OnSeagullHit += SeagullHitEvent;
        }

        private void OnDisable()
        {
            Seagull.OnSeagullHit -= SeagullHitEvent;
        }

        private void SeagullHitEvent()
        {
            AddScore(1);
        }

        private void Start()
        {
            RefreshUI();
            highScore = HighScore;
           
            
        }

        private void Update()
        {
            if (roundActive)
            {
                RoundTimer -= Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartRound();
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                AddScore(1);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                EndRound();
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                EndRound();
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
            roundStart.Play();
            
        }

        [ContextMenu("End Round")]
        public void EndRound()
        {
            Active = null;
            
            roundActive = false;
            RoundTimer = 0;
            roundEnd.Play();

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

        public void AddScore(int addedScore)
        {
            Score += addedScore;
        }
    }

    internal class seralisefieldAttribute : Attribute
    {
    }
}

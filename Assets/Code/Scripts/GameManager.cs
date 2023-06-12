using ShootingRangeGame.Scripts.Seagulls;
using TMPro;
using UnityEngine;

namespace ShootingRangeGame.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [Header("Round Settings")]
        public float roundLength;
        float roundTimer = 0;
        int score = 0;
        int highScore;
        bool roundActive = false;

        [Header("Components")]
        public TextMeshProUGUI timerReadout;
        public TextMeshProUGUI scoreReadout;
        public TextMeshProUGUI highScoreReadout;

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
            highScore = GetHighScore();
        }

        void Update()
        {
            if (roundActive)
            {
                roundTimer -= Time.deltaTime;
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

        public void StartRound()
        {
            roundTimer = roundLength;
            roundActive = true;
            score = 0;
        }

        public void EndRound()
        {
            roundActive = false;
            roundTimer = 0;

            SetHighScore(score);
        }

        public void RefreshUI()
        {
            if (roundActive)
            {
                timerReadout.text = (int)(roundTimer / 60) + ":" + (int)(roundTimer % 60);
            }

            scoreReadout.text = score.ToString();
            highScoreReadout.text = highScore.ToString();
        }

        public void AddScore(int addedScore)
        {
            score += addedScore;
        }

        public int GetHighScore()
        {
            return PlayerPrefs.GetInt("HighScore", 0);
        }

        public void SetHighScore(int score)
        {
            if (score > GetHighScore())
            {
                PlayerPrefs.SetInt("HighScore", score);
                highScore = score;
            }

        }


    }
}

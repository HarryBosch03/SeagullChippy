using TMPro;
using UnityEngine;

namespace ShootingRangeGame.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class ScoreUI : MonoBehaviour
    {
        GameManager manager;
        
        private TMP_Text text;
        private string template;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
            manager = FindObjectOfType<GameManager>();
            template = text.text;
        }

        private void Update()
        {
            text.text = string.Format(template, manager ? manager.Score : 0);
        }
    }
}

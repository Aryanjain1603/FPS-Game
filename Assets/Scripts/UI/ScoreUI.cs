using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Button startButton;
        
        private void Start()
        {
            ScoreManager.Instance.OnScoreChanged += UpdateUI;
            startButton.onClick.AddListener(OnClickStartButton);
        }

        private void OnClickStartButton()
        {
            ScoreManager.Instance.StartTimer();
            startButton.gameObject.SetActive(false);
        }

        private void UpdateUI(int _score1, int _score2)
        {
            scoreText.text = $"{_score1} killed , {_score2}";
          
        }
    }

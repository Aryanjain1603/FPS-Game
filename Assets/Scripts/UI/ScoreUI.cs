using System;
using Photon.Pun;
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
            if(!PhotonNetwork.IsMasterClient) startButton.gameObject.SetActive(false);
            ScoreManager.Instance.StartTimer();
            startButton.gameObject.SetActive(false);
        }

        private void UpdateUI(string nickName, int _score2)
        {
            scoreText.text = $"{nickName} kills :- {_score2}";
          
        }
    }

using TMPro;
using UnityEngine;

namespace UI
{
    public class MatchTImerUi : MonoBehaviour
    {
        public TextMeshProUGUI timerText;
        void Update()
        {
            if(ScoreManager.Instance == null) return;

            float time = ScoreManager.Instance.GetRemainingTime();
            int mins =  Mathf.FloorToInt(time / 60);
            int secs = Mathf.FloorToInt(time % 60);
            timerText.text = mins + ":" + secs;

        }
    }
}
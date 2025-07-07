using System;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthText;
    private void Start()
    {
        PlayerController.OnDamage += UpdateHealthUI;
    }

    private void OnDestroy()
    {
        PlayerController.OnDamage -= UpdateHealthUI;
    }
    
    private void UpdateHealthUI(int damage)
    {
        healthText.text = damage.ToString();
    }
}

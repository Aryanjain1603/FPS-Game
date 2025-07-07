using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] PhotonView photonView;
    [SerializeField] TextMeshProUGUI healthText;
    private void Start()
    {
        if(!photonView.IsMine) 
            GetComponent<Canvas>().enabled = false;
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

using System;
using Photon.Pun;
using UnityEngine;


    public class Enemy : MonoBehaviourPun,I_Damageable
    {
        public int health = 100;

        public void Damage(int damage)
        {
            if (health > damage)
            {
                health -= damage;
            }
            else 
            {
                gameObject.SetActive(false);
                PhotonNetwork.Destroy(gameObject);
            }
            Debug.Log(health);
        }
        [PunRPC]
        public void RPC_TakeDamage(int damage)
        {
            if (!photonView.IsMine) return;
            health -= damage;
            if (health <= 0)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

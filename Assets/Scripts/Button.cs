using UnityEngine;
using Photon.Pun;


    public class Button : MonoBehaviourPun,I_Interactable,I_Damageable
    {
        private bool isPressed = false;

        public int health = 100;
        public void Interact()
        {
            isPressed = !isPressed;
            Debug.Log("Button is pressed: " + isPressed);
            
        }

        public string GetInteractionText()
        {
            return "Press E to interact with the button ";
        }

        public void Damage(int damage)
        {
            if (health > damage)
            {
                health -= damage;
            }
            else
            {
                health = 0;
                gameObject.SetActive(false);
                PhotonNetwork.Destroy(gameObject);
            }
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

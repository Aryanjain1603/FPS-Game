using UnityEngine;


    public class Button : MonoBehaviour,I_Interactable,I_Damageable
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
            }
        }
    }

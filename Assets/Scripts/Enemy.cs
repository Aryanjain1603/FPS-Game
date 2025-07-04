using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Enemy : MonoBehaviour,I_Damageable
    {
        public int health = 100;

        public void Damage(int damage)
        {
            if (health > damage)
            {
                health -= damage;
            }
            else 
                gameObject.SetActive(false);
            Debug.Log(health);
        }
    }
}
using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float speed = 0f;
    private Rigidbody rb;
    // [SerializeField] private float speed = 1f;
    //
    //
    private void Awake()
    {
        // rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        // speed = rb.linearVelocity.magnitude;
        
    }
    private void OnCollisionEnter(Collision other)
    {
        I_Damageable damageable;
        damageable = other.gameObject.GetComponent<I_Damageable>();
        if (damageable != null)
        {
            damageable.Damage(30);
            Debug.Log("Bullet hit");
        }
    }
}

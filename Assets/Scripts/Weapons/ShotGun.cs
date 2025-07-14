using System.Collections;
using Player;
using UnityEngine;

public class Shotgun : BaseGun
{
    [Header("Shotgun Specific")]
    [SerializeField] private float pumpDelay = 0.5f;
    [SerializeField] private AudioClip pumpSound;
    [SerializeField] private bool isAutomatic = false;
    
    private bool isPumping = false;
    
    protected override void HandleInput()
    {
        base.HandleInput();
        
        // Shotgun fire mode
        if (Input.GetMouseButtonDown(0) )
        {
            TryShoot();
        }
        
        // Auto shotgun support
        if (isAutomatic && Input.GetMouseButton(0))
        {
            TryShoot();
        }
    }
    
    public override void TryShoot()
    {
        if (isPumping) return;
        
        base.TryShoot();
        
        // Start pump action delay
        if (!isAutomatic)
        {
            StartCoroutine(PumpAction());
        }
    }
    
    protected override void Shoot()
    {
        // Fire multiple pellets
        for (int i = 0; i < stats.pelletsPerShot; i++)
        {
            Vector3 shootDirection = GetShootDirection();
            FireBullet(cam.transform.position, shootDirection);
        }
    }
    
    private Vector3 GetShootDirection()
    {
        Vector3 direction = cam.transform.forward;
        
        // Apply wide spread for shotgun
        direction = ApplySpread(direction);
        
        return direction;
    }
    
    private IEnumerator PumpAction()
    {
        isPumping = true;
        
        yield return new WaitForSeconds(pumpDelay);
        
        // Play pump sound
        if (audioSource != null && pumpSound != null)
        {
            audioSource.PlayOneShot(pumpSound);
        }
        
        isPumping = false;
    }
    
    protected override bool CanShoot()
    {
        return base.CanShoot() && !isPumping;
    }
}
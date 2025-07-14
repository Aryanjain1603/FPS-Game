using UnityEngine;
using System.Collections;
using Player;

// ASSAULT RIFLE - Full automatic fire
public class AssaultRifle : BaseGun
{
    [Header("Assault Rifle Specific")]
    [SerializeField] private float recoilAccumulation = 0.02f;
    [SerializeField] private float recoilRecovery = 0.1f;
    [SerializeField] private float maxRecoil = 0.5f;
    
    private float currentRecoil = 0f;
    private bool isHoldingTrigger = false;
    
    protected override void HandleInput()
    {
        base.HandleInput();
        
        // Full-auto fire mode
        if (Input.GetMouseButtonDown(0) && !CursorLock.isCursorLock )
        {
            isHoldingTrigger = true;
            StartCoroutine(AutoFire());
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            isHoldingTrigger = false;
        }
    }
    
    private IEnumerator AutoFire()
    {
        while (isHoldingTrigger && !IsEmpty())
        {
            TryShoot();
            yield return new WaitForSeconds(1f / stats.fireRate);
        }
    }
    
    protected override void Shoot()
    {
        Vector3 shootDirection = GetShootDirection();
        FireBullet(cam.transform.position, shootDirection);
        
        // Accumulate recoil
        currentRecoil = Mathf.Min(currentRecoil + recoilAccumulation, maxRecoil);
        
        // Start recoil recovery
        StartCoroutine(RecoverFromRecoil());
    }
    
    private Vector3 GetShootDirection()
    {
        Vector3 direction = cam.transform.forward;
        
        // Apply spread based on current recoil
        float totalSpread = stats.spreadAngle + currentRecoil;
        direction = ApplySpread(direction, totalSpread);
        
        return direction;
    }
    
    private Vector3 ApplySpread(Vector3 direction, float spreadAmount)
    {
        if (spreadAmount <= 0) return direction;
        
        float spreadX = Random.Range(-spreadAmount, spreadAmount);
        float spreadY = Random.Range(-spreadAmount, spreadAmount);
        
        Vector3 spread = new Vector3(spreadX, spreadY, 0);
        return Quaternion.Euler(spread) * direction;
    }
    
    private IEnumerator RecoverFromRecoil()
    {
        yield return new WaitForSeconds(0.1f);
        
        while (currentRecoil > 0 && !isHoldingTrigger)
        {
            currentRecoil = Mathf.Max(0, currentRecoil - recoilRecovery * Time.deltaTime);
            yield return null;
        }
    }
    
    // protected override void HandleInput()
    // {
    //     base.HandleInput();
    //     
    //     // Stop auto fire when reloading
    //     if (isReloading)
    //     {
    //         isHoldingTrigger = false;
    //     }
    // }
}
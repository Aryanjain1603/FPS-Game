using UnityEngine;
using Photon.Pun;
using Player;

public class Pistol : BaseGun
{
    [Header("Pistol Specific")]
    [SerializeField] private bool allowRapidFire = false;
    [SerializeField] private float aimAssistRange = 2f;
    
    protected override void HandleInput()
    {
        base.HandleInput();
        
        // Pistol fire mode - semi-automatic (single shot per click)
        if (Input.GetMouseButtonDown(0))
        {
            TryShoot();
        }
        
        // Optional: Allow rapid fire if configured
        if (allowRapidFire && Input.GetMouseButton(0))
        {
            TryShoot();
        }
    }
    
    protected override void Shoot()
    {
        Vector3 shootDirection = GetShootDirection();
        FireBullet(cam.transform.position, shootDirection);
    }
    
    private Vector3 GetShootDirection()
    {
        Vector3 direction = cam.transform.forward;
        
        // Apply slight spread for realism
        direction = ApplySpread(direction);
        
        // Optional: Aim assist for closer targets
        if (aimAssistRange > 0)
        {
            direction = ApplyAimAssist(direction);
        }
        
        return direction;
    }
    
    private Vector3 ApplyAimAssist(Vector3 originalDirection)
    {
        Ray ray = new Ray(cam.transform.position, originalDirection);
        RaycastHit[] hits = Physics.RaycastAll(ray, stats.range, layerMask);
        
        foreach (RaycastHit hit in hits)
        {
            PhotonView targetPV = hit.collider.GetComponent<PhotonView>();
            if (targetPV != null && !targetPV.IsMine)
            {
                float distance = Vector3.Distance(cam.transform.position, hit.point);
                if (distance <= aimAssistRange)
                {
                    Vector3 aimDirection = (hit.point - cam.transform.position).normalized;
                    return Vector3.Lerp(originalDirection, aimDirection, 0.2f);
                }
            }
        }
        
        return originalDirection;
    }
}
using System.Collections;
using UnityEngine;

public class SniperRifle : BaseGun
{
    [Header("Sniper Specific")]
    
    [SerializeField] private float scopeZoom = 4f;
    [SerializeField] private float scopeTime = 0.5f;
    [SerializeField] private GameObject scopeOverlay;
    [SerializeField] private AudioClip boltSound;
    
    private bool isScoped = false;
    private float originalFOV;
    
    protected override void Start()
    {
        base.Start();
        originalFOV = cam.fieldOfView;
        
        if (scopeOverlay != null)
            scopeOverlay.SetActive(false);
    }
    
    protected override void HandleInput()
    {
        base.HandleInput();
        
        // Scope toggle
        if (Input.GetMouseButtonDown(1))
        {
            ToggleScope();
        }
        
        // Single shot fire mode
        if (Input.GetMouseButtonDown(0))
        {
            TryShoot();
        }
    }
    
    protected override void Shoot()
    {
        Vector3 shootDirection = cam.transform.forward;
        
        // Minimal spread when scoped
        if (isScoped)
        {
            shootDirection = ApplySpread(shootDirection, stats.spreadAngle * 0.1f);
        }
        else
        {
            shootDirection = ApplySpread(shootDirection);
        }
        
        FireBullet(cam.transform.position, shootDirection);
        
        // Play bolt action sound
        StartCoroutine(PlayBoltAction());
    }
    
    private void ToggleScope()
    {
        isScoped = !isScoped;
        StartCoroutine(ScopeTransition());
    }
    
    private IEnumerator ScopeTransition()
    {
        float targetFOV = isScoped ? originalFOV / scopeZoom : originalFOV;
        float startFOV = cam.fieldOfView;

        if (!isScoped && scopeOverlay != null)
            scopeOverlay.SetActive(false); // Hide immediately when unscoping

        float elapsed = 0f;
        while (elapsed < scopeTime)
        {
            elapsed += Time.deltaTime;
            cam.fieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsed / scopeTime);
            yield return null;
        }

        cam.fieldOfView = targetFOV;

        if (isScoped && scopeOverlay != null)
            scopeOverlay.SetActive(true); // Show only after delay when scoping in
    }

    
    private IEnumerator PlayBoltAction()
    {
        yield return new WaitForSeconds(0.2f);
        
        if (audioSource != null && boltSound != null)
        {
            audioSource.PlayOneShot(boltSound);
        }
    }
    
    private Vector3 ApplySpread(Vector3 direction, float customSpread = -1f)
    {
        float spread = customSpread >= 0 ? customSpread : stats.spreadAngle;
        
        if (spread <= 0) return direction;
        
        float spreadX = Random.Range(-spread, spread);
        float spreadY = Random.Range(-spread, spread);
        
        Vector3 spreadVector = new Vector3(spreadX, spreadY, 0);
        return Quaternion.Euler(spreadVector) * direction;
    }
}
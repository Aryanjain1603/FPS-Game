using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Player;
using TMPro;

[System.Serializable]
public class GunStats
{
    [Header("Damage & Range")]
    public int damage = 10;
    public float range = 100f;
    
    [Header("Fire Rate & Reload")]
    public float fireRate = 1f;
    public float reloadTime = 1f;
    
    [Header("Ammo")]
    public int clipSize = 30;
    public int maxClipSize = 30;
    public int ammo = 100;
    public int maxAmmo = 100;
    
    [Header("Accuracy")]
    public float accuracy = 1f;
    public float recoil = 0.1f;
    
    [Header("Bullet Physics")]
    public float bulletSpeed = 10f;
    public int pelletsPerShot = 1; // For shotguns
    public float spreadAngle = 0f; // For shotguns and automatic weapons
}

public abstract class BaseGun : MonoBehaviourPun
{
    [Header("References")]
    [SerializeField] protected Camera cam;
    [SerializeField] protected GameObject firePoint;
    [SerializeField] protected PhotonView photonView;
    [SerializeField] protected BulletPooling bulletPooling;
    
    [Header("Gun Stats")]
    [SerializeField] protected GunStats stats;
    
    [Header("UI")]
    [SerializeField] protected Image crosshair;
    [SerializeField] protected TextMeshProUGUI ammoText;
    
    [Header("Effects")]
    [SerializeField] protected ParticleSystem muzzleFlash;
    [SerializeField] protected GameObject hitEffectPrefab;
    [SerializeField] protected LineRenderer tracerPrefab;
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip shootSound;
    [SerializeField] protected AudioClip reloadSound;
    [SerializeField] protected AudioClip emptySound;
    
    [Header("Targeting")]
    [SerializeField] protected LayerMask layerMask;
    
    // State variables
    protected int currentAmmo;
    protected int currentClipAmmo;
    protected bool isReloading = false;
    protected bool canShoot = true;
    protected float nextFireTime = 0f;
    
    // Events
    public System.Action<int, int> OnAmmoChanged;
    public System.Action OnReloadStart;
    public System.Action OnReloadComplete;
    public System.Action OnShoot;
    
    protected virtual void Awake()
    {
        currentAmmo = stats.ammo;
        currentClipAmmo = stats.clipSize;
        
        if (photonView == null)
            photonView = GetComponent<PhotonView>();
            
        if (cam == null)
            cam = Camera.main;
    }
    
    protected virtual void Start()
    {
        UpdateAmmoUI();
    }
    
    protected virtual void Update()
    {
        if (!photonView.IsMine) return;
        
        HandleInput();
        UpdateAmmoUI();
    }
    
    protected virtual void HandleInput()
    {
        // Reload input
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentClipAmmo < stats.clipSize && currentAmmo > 0)
        {
            StartReload();
        }
        
        // Fire input - override in child classes for different fire modes
        if (Input.GetMouseButtonDown(0) && !CursorLock.isCursorLock )
        {
            TryShoot();
        }
    }
    
    public virtual void TryShoot()
    {
        if (!CanShoot()) return;
        
        if (currentClipAmmo <= 0)
        {
            PlayEmptySound();
            return;
        }
        
        Shoot();
        currentClipAmmo--;
        nextFireTime = Time.time + (1f / stats.fireRate);
        
        OnShoot?.Invoke();
    }
    
    protected virtual bool CanShoot()
    {
        return canShoot && !isReloading && Time.time >= nextFireTime && currentClipAmmo > 0;
    }
    
    protected abstract void Shoot();
    
    protected virtual void FireBullet(Vector3 origin, Vector3 direction)
    {
        Ray ray = new Ray(origin, direction);
        RaycastHit hitInfo;
        
        if (Physics.Raycast(ray, out hitInfo, stats.range, layerMask))
        {
            HandleHit(hitInfo);
        }
        
        // Visual effects
        PlayMuzzleFlash();
        PlayShootSound();
        
        // Spawn tracer effect
        if (tracerPrefab != null)
        {
            StartCoroutine(PlayBulletTrail(origin, hitInfo.collider ? hitInfo.point : origin + direction * stats.range));
        }
    }
    
    protected virtual void HandleHit(RaycastHit hitInfo)
    {
        // Damage target
        PhotonView targetPhotonView = hitInfo.collider.GetComponent<PhotonView>();
        if (targetPhotonView != null && !targetPhotonView.IsMine)
        {
            int damage = stats.damage;
            int attackerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            targetPhotonView.RPC("RPC_TakeDamage", RpcTarget.All, damage,attackerActorNumber);
        }
        
        // Spawn hit effect
        if (hitEffectPrefab != null)
        {
            GameObject hitEffect = Instantiate(hitEffectPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(hitEffect, 2f);
        }
    }
    
    public virtual void StartReload()
    {
        if (isReloading || currentAmmo <= 0) return;
        
        StartCoroutine(ReloadCoroutine());
    }
    
    protected virtual IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        OnReloadStart?.Invoke();
        PlayReloadSound();
        
        yield return new WaitForSeconds(stats.reloadTime);
        
        int ammoNeeded = stats.clipSize - currentClipAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, currentAmmo);
        
        currentClipAmmo += ammoToReload;
        currentAmmo -= ammoToReload;
        
        isReloading = false;
        OnReloadComplete?.Invoke();
    }
    
    protected virtual Vector3 ApplySpread(Vector3 direction)
    {
        if (stats.spreadAngle <= 0) return direction;
        
        float spreadX = UnityEngine.Random.Range(-stats.spreadAngle, stats.spreadAngle);
        float spreadY = UnityEngine.Random.Range(-stats.spreadAngle, stats.spreadAngle);
        
        Vector3 spread = new Vector3(spreadX, spreadY, 0);
        return Quaternion.Euler(spread) * direction;
    }
    
    protected virtual void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentClipAmmo}/{currentAmmo}";
        }
        
        OnAmmoChanged?.Invoke(currentClipAmmo, currentAmmo);
    }
    
    protected virtual void PlayMuzzleFlash()
    {
        if (muzzleFlash != null)
            muzzleFlash.transform.position = firePoint.transform.position;
            muzzleFlash.Play();
    }
    
    protected virtual void PlayShootSound()
    {
        if (audioSource != null && shootSound != null)
            audioSource.PlayOneShot(shootSound);
    }
    
    protected virtual void PlayReloadSound()
    {
        if (audioSource != null && reloadSound != null)
            audioSource.PlayOneShot(reloadSound);
    }
    
    protected virtual void PlayEmptySound()
    {
        if (audioSource != null && emptySound != null)
            audioSource.PlayOneShot(emptySound);
    }
    
    protected virtual IEnumerator PlayBulletTrail(Vector3 startPoint, Vector3 endPoint)
    {
        if (tracerPrefab == null) yield break;
        
        LineRenderer tracer = Instantiate(tracerPrefab);
        tracer.SetPosition(0, startPoint);
        tracer.SetPosition(1, endPoint);
        
        yield return new WaitForSeconds(0.1f);
        
        if (tracer != null)
            Destroy(tracer.gameObject);
    }
    
    // Utility methods
    public virtual void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Min(currentAmmo + amount, stats.maxAmmo);
    }
    
    public virtual bool IsEmpty()
    {
        return currentClipAmmo <= 0 && currentAmmo <= 0;
    }
    
    public virtual bool NeedsReload()
    {
        return currentClipAmmo <= 0 && currentAmmo > 0;
    }
    
    public virtual float GetReloadProgress()
    {
        return isReloading ? 1f - (stats.reloadTime) : 0f;
    }
    
    // Getters
    public int CurrentAmmo => currentAmmo;
    public int CurrentClipAmmo => currentClipAmmo;
    public bool IsReloading => isReloading;
    public GunStats Stats => stats;
}
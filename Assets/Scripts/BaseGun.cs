using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class BaseGun : MonoBehaviour
{
    [SerializeField] private Camera cam;
    public GameObject firePoint;

    public LineRenderer tracerPrefab;
    [SerializeField] private PhotonView photonView;



    public Image crosshair;
    public float bulletSpeed = 10f;
    public float fireRate = 1f;
    public int damage = 10;
    public float range = 100f;
    public float reloadTime = 1f;
    public int ammo = 100;
    public int maxAmmo = 100;
    public int clipSize = 100;
    public int maxClipSize = 100;
    public bool reloading = false;
    public LayerMask layerMask;
    [SerializeField] private BulletPooling bulletPooling;


    [Header("Effects")]
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject hitEffectPrefab;



    private void Awake()
    {

    }

    public void Update()
    {
        if (!photonView.IsMine) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        muzzleFlash.Play();
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        // StartCoroutine(PlayBulletTrail(cam.transform.position,cam.transform.forward * range));
        RaycastHit hitInfo;
        Debug.DrawRay(ray.origin, ray.direction * range, Color.red);
        if (Physics.Raycast(ray, out hitInfo, range))
        {
            PhotonView targetPhotonView = hitInfo.collider.GetComponent<PhotonView>();
            if (!targetPhotonView.IsMine && targetPhotonView != null)
            {
                targetPhotonView.RPC("RPC_TakeDamage", RpcTarget.All, damage);
            }
            // I_Damageable damageable = hitInfo.collider.GetComponent<I_Damageable>();
            // if (damageable != null)
            // {
            //     damageable.Damage(damage);
            //     //add effect at hit point
            // }
            GameObject hitEffect = Instantiate(hitEffectPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(hitEffect, 2f);
        }
    }

    private IEnumerator PlayBulletTrail(Vector3 start, Vector3 end)
    {
        LineRenderer lr = Instantiate(tracerPrefab);
        lr.SetPosition(0,start);   
        lr.SetPosition(1,start);
        
        float time = 0;
        float duration = 0.05f;
        while (time < duration)
        {
            lr.SetPosition(1, Vector3.Lerp(start, end, time / duration));
            time += Time.deltaTime;
            yield return null;
        }
        lr.SetPosition(1,end);
        Destroy(lr, 0.2f);
    }
}
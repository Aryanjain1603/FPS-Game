using System;
using Unity.VisualScripting;
using UnityEngine;


public class BaseGun : MonoBehaviour 
{
    public GameObject bulletPrefab;
    public GameObject firePoint;
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

    private void Start()
    {
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        Ray ray = new Ray(firePoint.transform.position, firePoint.transform.forward);
        RaycastHit hitInfo;
        Debug.DrawRay(firePoint.transform.position, firePoint.transform.forward * range, Color.red);
        if (Physics.Raycast(ray, out hitInfo, range, layerMask))
        {
            I_Damageable damageable = hitInfo.collider.GetComponent<I_Damageable>();
            if (damageable != null)
            {
                damageable.Damage(damage);
            }
        }
    }
    
}

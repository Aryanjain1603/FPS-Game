using System;
using System.Collections.Generic;
using UnityEngine;

public class BulletPooling : MonoBehaviour
{
    public List<GameObject> storedBullets;
    public List<GameObject> activeBullets;
    
    public GameObject  GetBullet()
    {
        GameObject bullet = storedBullets[0];
        storedBullets.RemoveAt(0);
        activeBullets.Add(bullet);
        return bullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        storedBullets.Add(bullet);
        activeBullets.Remove(bullet);
    }
    
}

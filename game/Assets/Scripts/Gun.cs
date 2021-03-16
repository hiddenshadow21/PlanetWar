using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public int fireRate = 1;
    public float reloadSpeed = 1f;
    public int maxAmmo = 15;

    private int ammo;
    public int Ammo { get { return ammo; } }
    public float nextShootTime;

    public GameObject weaponBullet;
    public Transform weaponFirePosition;

    private void Start()
    {
        nextShootTime = Time.time;
        ammo = maxAmmo;
    }
}

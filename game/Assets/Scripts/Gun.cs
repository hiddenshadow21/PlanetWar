using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : NetworkBehaviour
{
    [SerializeField]
    protected int fireRate = 1;
    public int FireRate { get { return fireRate; } }

    [SerializeField]
    protected float reloadTime = 1f;
    public float ReloadSpeed { get { return reloadTime; } }

    public int maxAmmo = 15;

    protected int ammo;
    public int Ammo { get { return ammo; } }
    public float nextShootTime;

    protected bool isReloading;

    public abstract void Shoot();

    public abstract void Reload();
}

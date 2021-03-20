using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunProjectile : Gun
{
    public GameObject weaponBullet;
    public Transform weaponFirePosition;

    [Server]
    public override void Reload()
    {
        if (isReloading)
            return;

        isReloading = true;
        StartCoroutine(ReloadCouritine());
        isReloading = false;

    }

    private IEnumerator ReloadCouritine()
    {
        yield return new WaitForSeconds(reloadTime);
        ammo = maxAmmo;
    }

    [Server]
    public override void Shoot()
    {
        if(isReloading)
            return;

        ammo--;
        var bullet = Instantiate(weaponBullet, weaponFirePosition.position, weaponFirePosition.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * weaponBullet.GetComponent<Bullet>().speed;
        NetworkServer.Spawn(bullet);
    }

    private void Start()
    {
        nextShootTime = Time.time;
        ammo = maxAmmo;
    }
}

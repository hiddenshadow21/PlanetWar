using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunProjectile : Gun
{
    public GameObject weaponBullet;

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
        if (isReloading)
            return;

        ammo--;
        var bullet = Instantiate(weaponBullet, weaponFirePosition.position, weaponFirePosition.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * weaponBullet.GetComponent<Bullet>().speed;
        NetworkServer.Spawn(bullet);

        RpcOnShoot();
    }

    [ClientRpc]
    private void RpcOnShoot()
    {

    }

    private void Start()
    {
        base.AssignToPlayer();
        Kolory kolor = transform.root.GetComponent<PlayerController>().Kolor;
        base.SetSprite(kolor);
        nextShootTime = Time.time;
        ammo = maxAmmo;
    }
}

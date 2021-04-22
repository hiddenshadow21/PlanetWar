using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunProjectile : Gun
{
    [SerializeField]
    private int force;

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
        bullet.GetComponent<Bullet>().SetShooterId(parentNetId);
        bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * weaponBullet.GetComponent<Bullet>().speed;
        NetworkServer.Spawn(bullet);
        AddForce(weaponFirePosition.rotation.normalized);
        RpcOnShoot();
    }

    [TargetRpc]
    private void AddForce(Quaternion rotation)
    {
        Vector2 v = new Vector2(-1,0);
        transform.root.GetComponent<Rigidbody2D>().AddForce(rotation * v * force);
    }

    [ClientRpc]
    private void RpcOnShoot()
    {

    }

    private void Awake()
    {
        
    }

    private void Start()
    {
        base.AssignToPlayer();
        Kolory kolor = transform.root.GetComponent<PlayerController>().Kolor;
        base.SetSprite(kolor);
        nextShootTime = Time.time;
        ammo = maxAmmo;
        force = 100;
    }
}

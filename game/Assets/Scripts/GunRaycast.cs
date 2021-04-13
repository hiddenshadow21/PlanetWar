using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRaycast : Gun
{
    [SerializeField]
    private float range = 100;
    [SerializeField]
    private float damage = 35;
    [SerializeField]
    private GameObject LaserLine;

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


        var hitInfo = Physics2D.Raycast(weaponFirePosition.position, weaponFirePosition.right, range);

        if (hitInfo)
        {
            var hitPlayer = hitInfo.transform.GetComponent<PlayerController>();
            if(hitPlayer != null)
            {
                hitPlayer.TakeDamage(damage);
            }
        }

        RpcOnShoot(hitInfo.point);
    }

    [ClientRpc]
    public void RpcOnShoot(Vector2 hitPoint)
    {
        var line = Instantiate(LaserLine);
        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, weaponFirePosition.position);
        if (hitPoint != Vector2.zero)
        {
            lineRenderer.SetPosition(1, hitPoint);
        }
        else
        {
            lineRenderer.SetPosition(1, weaponFirePosition.position + weaponFirePosition.right * range);

        }

        Destroy(line, 0.1f);
        //audio
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

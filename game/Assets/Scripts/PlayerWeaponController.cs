using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : NetworkBehaviour
{
    private Transform aimTransform;
    private int selectedWeaponLocal = 1;
    public Gun[] weapons;
    private Gun activeWeapon;

    [SyncVar(hook = nameof(OnWeaponChanged))]
    public int activeWeaponSynced = 1;

    void OnWeaponChanged(int _Old, int _New)
    {
        // disable old weapon
        // in range and not null
        if (_Old < weapons.Length && weapons[_Old] != null)
        {
            weapons[_Old].gameObject.SetActive(false);
        }

        // enable new weapon
        // in range and not null
        if (_New < weapons.Length && weapons[_New] != null)
        {
            weapons[_New].gameObject.SetActive(true);
            activeWeapon = weapons[_New];
        }
    }

    [Command]
    public void CmdChangeActiveWeapon(int newIndex)
    {
        activeWeaponSynced = newIndex;
    }

    private void Start()
    {
    }

    private void Awake()
    {
        aimTransform = transform.Find("Aim");
        foreach (var item in weapons)
        {
            if (item != null)
            {
                item.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;
        HandleAiming();
        HandleShooting();
        HandleWeaponSwitching();
    }

    private void HandleWeaponSwitching()
    {
        if (Input.GetButtonDown("Fire2")) //Fire2 is mouse 2nd click and left alt
        {
            selectedWeaponLocal += 1;

            if (selectedWeaponLocal >= weapons.Length)
            {
                selectedWeaponLocal = 0;
            }

            CmdChangeActiveWeapon(selectedWeaponLocal);
        }
    }

    private void HandleShooting()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            CmdCanShoot();
        }
    }

    [Command]
    private void CmdCanShoot()
    {
        if (Time.time >= activeWeapon.nextShootTime)
        {
            activeWeapon.nextShootTime = Time.time + 1.0f / activeWeapon.fireRate;
            RpcFireWeapon();
        }
    }

    [ClientRpc]
    void RpcFireWeapon()
    {
        var bullet = Instantiate(activeWeapon.weaponBullet, activeWeapon.weaponFirePosition.position, activeWeapon.weaponFirePosition.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * activeWeapon.weaponBullet.GetComponent<Bullet>().speed;
    }

    private void HandleAiming()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector3 aimDirection = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimTransform.eulerAngles = new Vector3(0, 0, angle);

        Vector3 aimLocalScale = Vector3.one;
        if(Quaternion.Angle(transform.rotation, aimTransform.rotation) > 90)
        {
            aimLocalScale.y = -1f;
        }
        else
        {
            aimLocalScale.y = 1f;
        }
        aimTransform.localScale = aimLocalScale;
    }
}


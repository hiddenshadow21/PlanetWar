using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerWeaponController : NetworkBehaviour
{
    private Transform aimTransform;
    private int selectedWeaponLocal = 0;
    private Gun[] weapons = new Gun[2];
    public Gun[] weaponsPrefabs;
    private Gun activeWeapon;
    public Transform hand;

    [SyncVar(hook = nameof(OnWeaponChanged))]
    public int activeWeaponSynced;


    void OnWeaponChanged(int _Old, int _New)
    {
        // disable old weapon
        // in range and not null
        if (_Old < weapons.Length && weapons[_Old] != null)
        {
            weapons[_Old].GetComponent<SpriteRenderer>().enabled = false;
        }

        // enable new weapon
        // in range and not null
        if (_New < weapons.Length && weapons[_New] != null)
        {
            weapons[_New].GetComponent<SpriteRenderer>().enabled = true;
            activeWeapon = weapons[_New];
        }
    }

    [Command]
    public void CmdChangeActiveWeapon(int newIndex)
    {
        if(activeWeapon != null)
            activeWeapon.GetComponent<SpriteRenderer>().enabled = false;
        activeWeaponSynced = newIndex;
        activeWeapon = weapons[newIndex];
        activeWeapon.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void Awake()
    {
        aimTransform = transform.Find("Aim");

    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        CmdSpawnSelectedWeapons();
        //CmdSpawnInactiveWeapons();
        CmdChangeActiveWeapon(0);
    }

    [Command]
    private void CmdSpawnInactiveWeapons()
    {
        foreach (var inactiveGun in FindObjectsOfType<Gun>().Where(x=> x.gameObject.activeSelf == false))
        {
            NetworkServer.Spawn(inactiveGun.gameObject);
        }
    }

    internal void SetGun(Gun gun)
    {
        if(weapons[0] == null)
        {
            weapons[0] = gun;
        }
        else
        {
            weapons[1] = gun;
        }
    }

    [Command]
    private void CmdSpawnSelectedWeapons()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject obj = Instantiate(weaponsPrefabs[i].gameObject, hand.position, hand.rotation) as GameObject;
            var gun = weapons[i] = obj.GetComponent<Gun>();
            obj.transform.SetParent(hand);
            gun.parentNetId = this.netId;
            NetworkServer.Spawn(obj, connectionToClient);
            //RpcSetParent(obj, gameObject, i);
        }
    }

    [ClientRpc]
    void RpcSetParent(GameObject obj, GameObject parent, int index)
    {
        var weaponCotroller = parent.GetComponent<PlayerWeaponController>();
        obj.transform.parent = weaponCotroller.hand;
        weaponCotroller.weapons[index] = obj.GetComponent<Gun>();
        obj.transform.localPosition = new Vector3(0, 0, 0);
        obj.transform.localRotation = new Quaternion(0, 0, 0, 1);
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;
        HandleAiming();
        HandleShooting();
        HandleWeaponSwitching();
        HandleReloading();
    }

    private void HandleReloading()
    {
        if (Input.GetButtonDown("Reload"))
        {
            CmdReload();
        }
    }

    [Command]
    private void CmdReload()
    {
        activeWeapon.Reload();
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
            CmdShoot();
        }
    }

    [Command]
    private void CmdShoot()
    {
        if (Time.time >= activeWeapon.nextShootTime && activeWeapon.Ammo > 0)
        {
            activeWeapon.nextShootTime = Time.time + 1.0f / activeWeapon.FireRate;

            activeWeapon.Shoot();
        }
        
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


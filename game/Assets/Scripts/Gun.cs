using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Kolory
{
    niebieski,
    zielony,
    pomarańczowy,
    fioletowy,
    czerwony
}

public abstract class Gun : NetworkBehaviour
{
    private HUD hud;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Sprite[] spriteColors;

    [SerializeField]
    protected int fireRate = 1;
    public int FireRate { get { return fireRate; } }

    [SerializeField]
    protected float reloadTime = 1f;
    public float ReloadSpeed { get { return reloadTime; } }

    public int maxAmmo = 15;

    [SyncVar(hook = nameof(Test))]
    protected int ammo;
    public int Ammo { get { return ammo; } }
    public float nextShootTime;

    protected bool isReloading;

    [SerializeField]
    protected Transform weaponFirePosition;

    [SyncVar]
    public uint parentNetId;

    public void Test(int _old, int _new)
    {
        if(hasAuthority)
        {
            hud.UpdateAmmo(_new, maxAmmo);
        }
        Debug.Log(_old + "->" + _new);
    }

    public abstract void Shoot();

    public abstract void Reload();

    protected void AssignToPlayer()
    {
        NetworkIdentity parentObject = FindObjectsOfType<NetworkIdentity>().Where(x => x.netId == parentNetId).FirstOrDefault();
        PlayerWeaponController playerWeaponController = parentObject.GetComponent<PlayerWeaponController>();
        transform.SetParent(playerWeaponController.hand);
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localRotation = Quaternion.identity;
        if (transform.localScale.y < 0)
        {
            transform.localScale += new Vector3(0, 2*Mathf.Abs(transform.localScale.y), 0);
        }
        playerWeaponController.SetGun(this);
        hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUD>();
        if (hasAuthority)
        {
            hud.UpdateAmmo(Ammo, maxAmmo);
        }
    }

    public void SetSprite(Kolory kolor)
    {
        spriteRenderer.sprite = spriteColors[(int)kolor];
    }
}

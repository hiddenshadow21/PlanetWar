using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float fireRate = 40f;
    public float reloadSpeed = 1f;
    public int maxAmmo = 15;

    public GameObject weaponBullet;
    public Transform weaponFirePosition;
}

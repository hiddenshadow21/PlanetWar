using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSpeedChanger : Bonus
{
    [Server]
    public override void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isServer)
            return;

        Debug.Log(collider.gameObject.name);

        var playerWeaponController = collider.gameObject.GetComponent<PlayerController>().gameObject.GetComponent<PlayerWeaponController>();
        if (playerWeaponController == null)
            return;

        playerWeaponController.StartChangeLaserSpeedBonus(10);
        OnPlayerTakeBonus();
    }
}

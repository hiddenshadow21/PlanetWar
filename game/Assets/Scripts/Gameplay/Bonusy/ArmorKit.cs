using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorKit : Bonus
{
    [Server]
    public override void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isServer)
            return;

        Debug.Log(collider.gameObject.name);

        var player = collider.gameObject.GetComponent<PlayerController>();
        if (player == null)
            return;

        if (player.armor != player.maxArmor)
        {
            player.AddArmor(RestoreAmount);
            OnPlayerTakeBonus();
        }
    }
}

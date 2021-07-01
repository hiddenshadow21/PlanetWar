using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedKit : Bonus
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

        if (player.health != player.maxHealth)
        {
            player.AddHealth(RestoreAmount);
            OnPlayerTakeBonus();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PoisonAreaShield : Bonus
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

        player.StartPoisonAreaShield(20f);
        OnPlayerTakeBonus();
    }
}

using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedKit : BonusBase
{

    [SerializeField]
    private float RestoreAmount = 10;

    [Server]
    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isServer)
            return;

        Debug.Log(collider.gameObject.name);

        var player = collider.gameObject.GetComponent<PlayerController>();
        if (player == null)
            return;

        player.AddHealth(RestoreAmount);
        NetworkServer.Destroy(gameObject);
    }

    protected override void Start()
    {
        base.Start();
    }
}

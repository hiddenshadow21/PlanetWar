using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Bonus : NetworkBehaviour
{
    public event EventHandler<int> onPlayerTakeBonus;

    [SerializeField]
    public float RestoreAmount { get; set; } = 10;

    public int Id { get; set; }

    protected void OnPlayerTakeBonus()
    {
        onPlayerTakeBonus?.Invoke(gameObject, Id);
    }

    [Server]
    public virtual void OnTriggerEnter2D(Collider2D collider)
    {
    }
}

using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BonusBase : NetworkBehaviour
{
    [SerializeField]
    protected float LifeTime = 10;

    [Server]
    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        return;
    }

    protected IEnumerator DestroyCoroutine(float t)
    {
        yield return new WaitForSeconds(t);
        NetworkServer.Destroy(gameObject);
    }

    protected virtual void Start()
    {
        StartCoroutine(DestroyCoroutine(LifeTime));
    }
}

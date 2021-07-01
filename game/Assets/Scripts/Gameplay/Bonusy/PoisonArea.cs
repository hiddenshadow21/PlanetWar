using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PoisonArea : NetworkBehaviour
{
    private Coroutine decreasePlayersHpCoroutine;
    private bool forwardModeOn = true;
    private readonly List<PlayerController> enteredPlayers = new List<PlayerController>();
    public event EventHandler<int> onPoisonAreaDestroy;
    int mode = 0; //0 - forward, 1 - waiting, 2 - backward, 3 - destroy
    public int Id { get; set; }

    [Server]
    private void Start()
    {
        StartCoroutine(resizeArea(0.1f));
        decreasePlayersHpCoroutine = StartCoroutine(decreasePlayersHp());
    }

    [Server]
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isServer)
            return;

        var player = collider.gameObject.GetComponent<PlayerController>();

        if (player == null)
            return;
        Debug.Log(collider.gameObject.name + " enter!");

        enteredPlayers.Add(player);
    }

    [Server] 
    public void OnTriggerExit2D(Collider2D collider)
    {
        if (!isServer)
            return;

        var player = collider.gameObject.GetComponent<PlayerController>();

        if (player == null)
            return;
        Debug.Log(collider.gameObject.name + " leave!");

        StopCoroutine(decreasePlayersHpCoroutine);

        enteredPlayers.Remove(player);

        decreasePlayersHpCoroutine = StartCoroutine(decreasePlayersHp());
    }

    [Server]
    private IEnumerator decreasePlayersHp()
    {
        foreach(PlayerController pc in enteredPlayers)
        {
            if(pc.IsPoisonAreaShieldActive == false)
                pc.TakeDamage(5, 10);
        }

        yield return new WaitForSeconds(1f);
        decreasePlayersHpCoroutine = StartCoroutine(decreasePlayersHp());
    }

    [Server]
    private IEnumerator resizeArea(float updateTime)
    {
        yield return new WaitForSeconds(updateTime);
        var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        switch(mode)
        {
            case 0:
                spriteRenderer.transform.localScale += new Vector3(0.008f, 0.008f, 0);
                if (spriteRenderer.transform.localScale.x > 1.8f)
                    mode = 1;
                StartCoroutine(resizeArea(0.15f));
                break;
            case 1:
                mode = 2;
                StartCoroutine(resizeArea(4f));
                break;
            case 2:
                spriteRenderer.transform.localScale -= new Vector3(0.008f, 0.008f, 0);
                if (spriteRenderer.transform.localScale.x < 0.6f)
                    mode = 3;
                StartCoroutine(resizeArea(0.15f));
                break;
            case 3:
                onPoisonAreaDestroy?.Invoke(gameObject, Id);
                break;
        }
    }

    [Server]
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 15f * Time.deltaTime));
    }
}

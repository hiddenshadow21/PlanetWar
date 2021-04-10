using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRespawnSystem : NetworkBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private Transform[] spawnPoints;

    [SerializeField]
    private Button spawnButton;
    [SerializeField]
    private Canvas canvas;

    public void SpawnPlayer()
    {
        ClientScene.AddPlayer(connectionToClient);
        canvas.gameObject.SetActive(false);
        //var playerGO = Instantiate<GameObject>(playerPrefab, spawnPoints[0].position, spawnPoints[0].rotation);
        //NetworkServer.Spawn(playerGO, connectionToClient);
    }
}

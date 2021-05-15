using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarpetBombing : Bonus
{
    public GameObject Meteorite;
    private BonusSpawnPoint[] spawnPoints;

    [Server]
    public override void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isServer)
            return;

        var player = collider.gameObject.GetComponent<PlayerController>();

        if (player == null)
            return;

        var spawnPointsContainer = GameObject.FindGameObjectWithTag("CarpetBombingSpawnPoints");
        spawnPoints = spawnPointsContainer.GetComponentsInChildren<BonusSpawnPoint>();

        OnPlayerTakeBonus();

        var roomManager = NetworkManager.singleton as NetworkRoomManagerExt;
        foreach (var nc in roomManager.gamePlayers)
        {
            nc.TargetSendInfoAboutCarpetBombing(player.playerName);
        }

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject meteorite = Instantiate(Meteorite, spawnPoints[i].Coordinates, new Quaternion(0f, 0f, -0.383f, 0.924f));
            meteorite.GetComponent<Bullet>().SetShooterId(player.netId);
            meteorite.GetComponent<Rigidbody2D>().velocity = meteorite.transform.right * meteorite.GetComponent<Bullet>().speed;
            NetworkServer.Spawn(meteorite);
        }
    }
}

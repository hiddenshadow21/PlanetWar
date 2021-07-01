using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarpetBombing : Bonus
{
    public GameObject Meteorite;
    private BonusSpawnPoint[] spawnPoints;
    public SpriteRenderer sprite;

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
        player.RpcSendInfoAboutCarpetBombing(player.playerName);

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject meteorite = Instantiate(Meteorite, spawnPoints[i].Coordinates, new Quaternion(0f, 0f, -0.383f, 0.924f));

            meteorite.GetComponent<Meteorite>().color = getPlayerColor(player.Kolor);
            meteorite.GetComponent<Bullet>().SetShooterId(player.netId);
            meteorite.GetComponent<Rigidbody2D>().velocity = meteorite.transform.right * meteorite.GetComponent<Bullet>().speed;

            NetworkServer.Spawn(meteorite);
        }
    }

    [Server]
    private Color getPlayerColor(Kolory color)
    {
        switch (color)
        {
            case Kolory.niebieski:
                return new Color(0, .7f, 1f);
            case Kolory.zielony:
                return new Color(.5f, 1f, .5f);
            case Kolory.pomarańczowy:
                return new Color(1f, .55f, .35f);
            case Kolory.fioletowy:
                return new Color(1f, .5f, 1f);
            case Kolory.czerwony:
                return new Color(1f, 0f, 0f, 1f);
            default:
                return new Color(1f, 1f, 1f, 1f);
        }
    }
}

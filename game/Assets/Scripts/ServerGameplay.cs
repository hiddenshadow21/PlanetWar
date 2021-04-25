using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ServerGameplay : NetworkBehaviour
{
    public int MatchTime;
    NetworkRoomManagerExt roomManager;

    void Start()
    {
        roomManager = NetworkManager.singleton as NetworkRoomManagerExt;
        StartCoroutine(updateMatchTimeServer());
    }

    [Server]
    private IEnumerator updateMatchTimeServer()
    {
        yield return new WaitForSeconds(1f);
        if (MatchTime > 0)
        {
            MatchTime--;
            UpdateMatchTimeClients();
            StartCoroutine(updateMatchTimeServer());
        }
        else
        {
            // TO DO
            // Koniec meczu
        }
    }

    [Server]
    private void UpdateMatchTimeClients()
    {
        roomManager.gamePlayers[0].RpcUpdateHudTimer(MatchTime);
    }

}

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
            foreach (var gP in roomManager.gamePlayers)
                gP.DisableComponents();
            // tutaj wyświetl tabelkę wyników
            // Po 5s przechodzi do pokoju
            StartCoroutine(endMatchWithDelay(5));
        }
    }

    [Server]
    private void UpdateMatchTimeClients()
    {
        roomManager.gamePlayers[0].RpcUpdateHudTimer(MatchTime);
    }

    [Server]
    private IEnumerator endMatchWithDelay(float t)
    {
        yield return new WaitForSeconds(t);
        roomManager.ServerChangeScene(roomManager.RoomScene);
    }

}

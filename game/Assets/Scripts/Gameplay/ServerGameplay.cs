using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ServerGameplay : NetworkBehaviour
{
    private const int chatNumber = 8;
    public int MatchTime;
    public List<string> Chats = new List<string>();
    private bool isChatUpdateRunning = true;
    NetworkRoomManagerExt roomManager;

    void Start()
    {
        roomManager = NetworkManager.singleton as NetworkRoomManagerExt;
        StartCoroutine(updateMatchTimeServer());
        initChatMessages();
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
        foreach (var nc in roomManager.gamePlayers)
        {
            nc.TargetUpdateHudTimer(nc.connectionToClient, MatchTime);
        }
    }

    [Server]
    private IEnumerator endMatchWithDelay(float t)
    {
        yield return new WaitForSeconds(t);
        roomManager.ServerChangeScene(roomManager.RoomScene);
    }

    #region Chat methods
    [Server]
    private void initChatMessages()
    {
        for (int i = 0; i < chatNumber; i++)
        {
            Chats.Add("");
        }
        StartCoroutine(updateChatPositionPeriodically());
    }

    [Server]
    public void SendChatMessage(string username, string message)
    {
        if(username != "" && message != "")
        {
            isChatUpdateRunning = false;
            string chatFormat = username + "~" + message;
            updateChatPosition();
            Chats[0] = chatFormat;
            sendUpdateToClients();
        }
    }

    [Server]
    private IEnumerator updateChatPositionPeriodically()
    {
        while (true)
        {
            if (isChatUpdateRunning == false)
            {
                isChatUpdateRunning = true;
            }
            else
            {
                updateChatPosition();
                sendUpdateToClients();
            }
            yield return new WaitForSeconds(15);
        }   
    }

    [Server]
    private void updateChatPosition()
    {
        for (int i = chatNumber - 1; i > 0; i--)
        {
            Chats[i] = Chats[i - 1];
        }
        Chats[0] = "";
    }

    [Server]
    private void sendUpdateToClients()
    {
        foreach (var nc in roomManager.gamePlayers)
        {
            nc.TargetUpdateChat(nc.connectionToClient, Chats);
        }
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class ServerGameplay : NetworkBehaviour
{
    private const int chatNumber = 8;
    public int MatchTime;
    public List<string> Chats = new List<string>();
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

    [Server]
    private void initChatMessages()
    {
        for(int i = 0; i < chatNumber; i++)
        {
            Chats.Add("");
        }
        InvokeRepeating(nameof(updateChatPosition), 0, 15f);
    }

    [Server]
    public void SendChatMessage(string username, string message)
    {
        string chatFormat = username + "~" + message;
        CancelInvoke(nameof(updateChatPosition));
        updateChatPosition(false);
        Chats[0] = chatFormat;
        //roomManager.gamePlayers[0].UpdateChat(gamePlayerChats);
        foreach(var nc in roomManager.gamePlayers)
        {
            nc.TargetUpdateChat(nc.connectionToClient, Chats);
        }
        InvokeRepeating(nameof(updateChatPosition), 0, 15f);
    }

    [Server]
    private void updateChatPosition(bool updateClients = true)
    {
        for (int i = chatNumber - 1; i > 0; i--)
        {
            Chats[i] = Chats[i - 1];
        }
        Chats[0] = "";

        if(updateClients == true)
        {
            foreach (var nc in roomManager.gamePlayers)
            {
                nc.TargetUpdateChat(nc.connectionToClient, Chats);
            }
        }
    }
}

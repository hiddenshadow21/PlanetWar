using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

[AddComponentMenu("")]
public class NetworkRoomManagerExt : NetworkRoomManager
{
    public string Key { get; set; }
    public ServerGameplay serverGameplay;
    public List<PlayerController> gamePlayers = new List<PlayerController>();

    /// <summary>
    /// Called just after GamePlayer object is instantiated and just before it replaces RoomPlayer object.
    /// This is the ideal point to pass any data like player name, credentials, tokens, colors, etc.
    /// into the GamePlayer object as it is about to enter the Online scene.
    /// </summary>
    /// <param name="roomPlayer"></param>
    /// <param name="gamePlayer"></param>
    /// <returns>true unless some code in here decides it needs to abort the replacement</returns>
    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnection conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        var rP = roomPlayer.GetComponent<NetworkRoomPlayerExt>();
        var gP = gamePlayer.GetComponent<PlayerController>();

        gamePlayers.Add(gP);

        gP.playerName = rP.PlayerName;

        switch (rP.index)
        {
            case 0:
                gP.Kolor = Kolory.niebieski;
                break;
            case 1:
                gP.Kolor = Kolory.zielony;
                break;
            case 2:
                gP.Kolor = Kolory.pomarañczowy;
                break;
            case 3:
                gP.Kolor = Kolory.fioletowy;
                break;
            case 4:
                gP.Kolor = Kolory.czerwony;
                break;
            default:
                gP.Kolor = Kolory.niebieski;
                break;
        }

        if (gamePlayers.Count == roomSlots.Count)
            Instantiate(serverGameplay);

        return true;
    }

    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
    {
        base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
        if (newSceneName != RoomScene)
        {
            foreach (NetworkRoomPlayerExt item in roomSlots)
            {
                item.Panel.SetActive(false);
            }
        }
        else
        {
            foreach (NetworkRoomPlayerExt item in roomSlots)
            {
                item.Panel.SetActive(true);
            }
        }
    }

    public override void OnRoomStopClient()
    {
        // Demonstrates how to get the Network Manager out of DontDestroyOnLoad when
        // going to the offline scene to avoid collision with the one that lives there.
        if (gameObject.scene.name == "DontDestroyOnLoad" && !string.IsNullOrEmpty(offlineScene) && SceneManager.GetActiveScene().path != offlineScene)
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());

        base.OnRoomStopClient();
    }

    public override void OnRoomStopServer()
    {
        // Demonstrates how to get the Network Manager out of DontDestroyOnLoad when
        // going to the offline scene to avoid collision with the one that lives there.
        if (gameObject.scene.name == "DontDestroyOnLoad" && !string.IsNullOrEmpty(offlineScene) && SceneManager.GetActiveScene().path != offlineScene)
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());

        base.OnRoomStopServer();
    }

    public override void OnRoomStartServer()
    {

        base.OnRoomStartServer();
    }

    /*
        This code below is to demonstrate how to do a Start button that only appears for the Host player
        showStartButton is a local bool that's needed because OnRoomServerPlayersReady is only fired when
        all players are ready, but if a player cancels their ready state there's no callback to set it back to false
        Therefore, allPlayersReady is used in combination with showStartButton to show/hide the Start button correctly.
        Setting showStartButton false when the button is pressed hides it in the game scene since NetworkRoomManager
        is set as DontDestroyOnLoad = true.
    */

    public override void OnRoomServerPlayersReady()
    {
        // calling the base method calls ServerChangeScene as soon as all players are in Ready state.

    #if UNITY_SERVER
        base.OnRoomServerPlayersReady();
    #endif
    }

    public override void OnGUI()
    {
        // do not show
    }

    public override void OnRoomClientDisconnect(NetworkConnection conn) 
    {
        // TO DO - exit
        // wraca do mainmenu
    }

    public override void OnRoomServerDisconnect(NetworkConnection conn)
    {
        base.OnRoomServerDisconnect(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // znajduje obiekt gracza po po³¹czeniu i usuwa z listy
        if (SceneManager.GetActiveScene().name == GameplayScene)
        {
            var rP = FindObjectsOfType<PlayerController>().Where(x => x.connectionToClient == conn).FirstOrDefault();
            Debug.Log("---Wyszedl gracz o nazwie: " + rP.playerName);
            gamePlayers.Remove(rP);
        }

        base.OnServerDisconnect(conn);
        if (roomSlots.Count == 0)
        {
            Application.Quit();
        }
    }
}
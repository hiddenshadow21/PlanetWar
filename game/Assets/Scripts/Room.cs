using Mirror;
using Mirror.Examples.NetworkRoom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public Button ButtonReadyState;
    public Button ButtonExitRoom;
    public Button ButtonStartGame;
    public Text TextRoomKey;

    NetworkRoomManagerExt networkRoomManagerExt;
    NetworkRoomPlayerExt networkRoomPlayerExt;


    void Start()
    {
        changeShowGuiStatus(false);
        networkRoomManagerExt = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<NetworkRoomManagerExt>();
        StartCoroutine(GetNetworkRoomPlayerExtSingleton(2));
        TextRoomKey.text += networkRoomManagerExt.Key;
    }

    IEnumerator GetNetworkRoomPlayerExtSingleton(float time)
    {
        yield return new WaitForSeconds(time);
        networkRoomPlayerExt = NetworkRoomPlayerExt.singleton;
        changeShowGuiStatus(true);
    }

    private void changeShowGuiStatus(bool status)
    {
        ButtonReadyState.enabled = status;
        ButtonExitRoom.enabled = status;
        ButtonStartGame.enabled = status;
    }

    private void OnEnable()
    {
        ButtonReadyState.onClick.AddListener(ChangeReadyState);
        ButtonExitRoom.onClick.AddListener(ExitRoom);
        ButtonStartGame.onClick.AddListener(StartGame);
    }

    private void ChangeReadyState()
    {
        Debug.Log("networkRoomPlayerExt.readyToBegin = " + networkRoomPlayerExt.readyToBegin);
        if (networkRoomPlayerExt.readyToBegin)
        {
            ButtonReadyState.GetComponentInChildren<Text>().text = "Ready";  
        }
        else
        {
            ButtonReadyState.GetComponentInChildren<Text>().text = "Not ready";
        }
        networkRoomPlayerExt.CmdChangeReadyState(!networkRoomPlayerExt.readyToBegin);
    }

    private void StartGame()
    {
        if (networkRoomManagerExt.allPlayersReady)
        {
            changeShowGuiStatus(false);
            networkRoomManagerExt.ServerChangeScene(networkRoomManagerExt.GameplayScene);
        }
    }

    // TO DO - fix
    private void ExitRoom()
    {
        networkRoomManagerExt.GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
    }

}

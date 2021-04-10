﻿using kcp2k;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject RoomManager;

    public Button ButtonCreateRoom;
    public Button ButtonShowJoinPanel;
    public Button ButtonExitGame;
    public Button ButtonBackFromJoin;
    public Button ButtonJoinRoom;
    public InputField InputRoomKey;
    public Text TextPlayerName;

    public GameObject JoinPanel;

    KcpTransport kcpTransport;
    NetworkRoomManagerExt networkRoomManagerExt;

    private const string networkAddress = "40.69.215.163";
    private ushort port;
    private string key;

    void Awake()
    {
        kcpTransport = RoomManager.GetComponent<KcpTransport>();
        networkRoomManagerExt = RoomManager.GetComponent<NetworkRoomManagerExt>();
        TextPlayerName.text = PlayerInfo.Name;
    }

    private void OnEnable()
    {
        ButtonCreateRoom.onClick.AddListener(CreateRoom);
        ButtonShowJoinPanel.onClick.AddListener(ShowJoinPanel);
        ButtonJoinRoom.onClick.AddListener(JoinRoom);
        ButtonBackFromJoin.onClick.AddListener(HideJoinPanel);
        ButtonExitGame.onClick.AddListener(ExitGame);
    }

    void OnGUI()
    {
        if (NetworkClient.isConnected && !ClientScene.ready)
        {
            ClientScene.Ready(NetworkClient.connection);

            if (ClientScene.localPlayer == null)
            {
                ClientScene.AddPlayer(NetworkClient.connection);
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    // TO DO - popraw kod bo jest prawie taki sam jak join
    public void CreateRoom()
    {
        generateRandomKey();
        networkRoomManagerExt.networkAddress = networkAddress;
        networkRoomManagerExt.Key = key;
        StartCoroutine(createRoomPHP());
    }
    public void JoinRoom()
    {
        key = InputRoomKey.text;
        networkRoomManagerExt.networkAddress = networkAddress;
        networkRoomManagerExt.Key = key;
        StartCoroutine(joinRoomPHP());
    }
    private IEnumerator createRoomPHP()
    {
        var form = new WWWForm();
        form.AddField("key", key);

        var www = new WWW($"http://{networkAddress}/roomManager/createRoomByKey.php", form);
        yield return www;

        switch (www.text)
        {
            case "KeyIsEmpty":
                // TO DO - show error
                break;
            case "KeyLengthError":
                // TO DO - show error
                break;
            case "ThisKeyAlreadyExists":
                // TO DO - show error
                // repeat 
                break;
            default:
                port = Convert.ToUInt16(www.text);
                kcpTransport.Port = port;
                networkRoomManagerExt.StartClient();
                break;
        }
        
    }

    private IEnumerator joinRoomPHP()
    {
        var form = new WWWForm();
        form.AddField("key", key);

        var www = new WWW($"http://{networkAddress}/roomManager/findRoomByKey.php", form);
        yield return www;

        switch (www.text)
        {
            case "KeyIsEmpty":
                // TO DO - show error
                Debug.Log("!! KeyIsEmpty !!");
                break;
            case "KeyLengthError":
                // TO DO - show error
                Debug.Log("!! KeyLengthError !!");
                break;
            case "KeyNotFound":
                // TO DO - show error
                Debug.Log("!! KeyNotFound !!");
                break;
            default:
                port = Convert.ToUInt16(www.text);
                kcpTransport.Port = port;
                networkRoomManagerExt.StartClient();
                break;
        }

    }
    private void generateRandomKey()
    {
        const int N = 5;
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new System.Random();

        for (int i = 0; i < N; i++)
            key += chars[random.Next(chars.Length)];
    }

    private void ShowJoinPanel()
    {
        JoinPanel.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
    private void HideJoinPanel()
    {
        JoinPanel.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}

using kcp2k;
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
    public Button ButtonJoinRoom;
    public InputField InputRoomKey;
    public Text TextPlayerName;

    KcpTransport kcpTransport;
    NetworkRoomManagerExt manager;

    private const string networkAddress = "40.69.215.163";
    private ushort port = 7000;
    private string key = "";

    void Awake()
    {
        kcpTransport = RoomManager.GetComponent<KcpTransport>();
        manager = RoomManager.GetComponent<NetworkRoomManagerExt>();
        TextPlayerName.text = PlayerInfo.Name;
    }

    private void OnEnable()
    {
        ButtonCreateRoom.onClick.AddListener(CreateRoom);
        ButtonJoinRoom.onClick.AddListener(JoinRoom);
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
        // TO DO - exit game
    }

    // TO DO - popraw kod
    public void CreateRoom()
    {
        generateRandomKey();
        StartCoroutine(createRoomPHP());
        kcpTransport.Port = port;
        manager.networkAddress = networkAddress;
        manager.StartClient();
    }
    public void JoinRoom()
    {
        key = InputRoomKey.text;
        StartCoroutine(joinRoomPHP());
        kcpTransport.Port = port;
        manager.networkAddress = networkAddress;
        manager.StartClient();
    }
    private IEnumerator createRoomPHP()
    {
        var form = new WWWForm();
        form.AddField("key", key);

        var www = new WWW($"http://{networkAddress}/createRoomByKey.php", form);
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
                Debug.Log(www.text);
                port = Convert.ToUInt16(www.text);
                break;
        }
    }

    private IEnumerator joinRoomPHP()
    {
        var form = new WWWForm();
        form.AddField("key", key);

        var www = new WWW($"http://{networkAddress}/findRoomByKey.php", form);
        yield return www;

        switch (www.text)
        {
            case "KeyIsEmpty":
                // TO DO - show error
                break;
            case "KeyLengthError":
                // TO DO - show error
                break;
            case "KeyNotFound":
                // TO DO - show error
                break;
            default:
                port = Convert.ToUInt16(www.text);
                kcpTransport.Port = port;
                manager.networkAddress = networkAddress;
                manager.StartClient();
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
}

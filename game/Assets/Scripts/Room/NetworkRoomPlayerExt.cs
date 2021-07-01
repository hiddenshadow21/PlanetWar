using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;


[AddComponentMenu("")]
public class NetworkRoomPlayerExt : NetworkRoomPlayer
{
    // UI Elements

    public Image ImagePlayerColor;
    public Text TextPlayerName;
    public Text TextReadyState;
    public GameObject Panel;

    public Button ButtonKick;

    /// NetworkRoomPlayerExt singleton
    public static NetworkRoomPlayerExt singleton { get; private set; }

    static readonly ILogger logger = LogFactory.GetLogger(typeof(NetworkRoomPlayerExt));
    public NetworkRoomManagerExt roomManager;

    [SyncVar]
    public int MatchTime;

    [SyncVar(hook = nameof(PlayerNameChanged))]
    public string PlayerName;


    [SyncVar(hook = nameof(PlayerColorChanged))]
    public Color32 PlayerColor;

    #region Commands

    [Command]
    public void CmdChangePlayerName(string playerName)
    {
        roomManager = NetworkManager.singleton as NetworkRoomManagerExt;
        foreach (NetworkRoomPlayerExt item in roomManager.roomSlots)
        {
            if (item == this)
            {
                item.PlayerName = playerName;
            }
        }
    }

    [Command]
    public void CmdChangeMatchTime(int time)
    {
        roomManager = NetworkManager.singleton as NetworkRoomManagerExt;
        foreach (NetworkRoomPlayerExt item in roomManager.roomSlots)
        {
            if (item == this)
            {
                item.MatchTime = time;
            }
        }
    }

    [Command]
    public void CmdChangePlayerColor()
    {
        roomManager = NetworkManager.singleton as NetworkRoomManagerExt;
        foreach (NetworkRoomPlayerExt item in roomManager.roomSlots)
        {
            if (item == this)
            {
                Debug.Log("---item.index = " + item.index);
                Color32 color = getPlayerColor(item.index);
                item.PlayerColor = color;
                item.ImagePlayerColor.color = color;
            }
        }
    }

    public void CmdKickPlayer(int indexToKick)
    {
        roomManager.roomSlots.RemoveAll(s => s.index == indexToKick);
    }
    #endregion

    #region SyncVar Hooks

    private void PlayerNameChanged(string oldName, string newName)
    {
        TextPlayerName.text = newName;
    }

    private void PlayerColorChanged(Color32 oldColor, Color32 newColor)
    {
        ImagePlayerColor.color = newColor;
    }

    public override void IndexChanged(int oldIndex, int newIndex) 
    {
        CmdChangePlayerColor();
        updatePositions();
        //updateButtonKick();
        if (isLocalPlayer)
        {
            if (newIndex == 0)
            {
                CmdChangeMatchTime(180);
                ButtonKick.gameObject.SetActive(true);
            }
        }
    }

    public override void ReadyStateChanged(bool _, bool newReadyState)
    {
        Debug.Log("---Hook ReadyStateChanged---");
        if (newReadyState == true)
        {
            TextReadyState.text = "Ready";
            TextReadyState.color = new Color32(0, 183, 0, 255);
        }
        else
        {
            TextReadyState.text = "Not ready";
            TextReadyState.color = new Color32(183, 0, 0, 255);
        }
    }

    #endregion

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    public override void OnStartClient()
    {
        Debug.Log("---OnStartClient()---");
        base.OnStartClient();
    }

    public override void OnClientEnterRoom()
    {
        Debug.Log("---OnClientEnterRoom()---");
        showRoomGUI = false;
        CmdChangePlayerName(PlayerInfo.Name);
        CmdChangePlayerColor();
        CmdChangeMatchTime(180);
        updatePositions();
        updateButtonKick();
        ButtonKick.gameObject.SetActive(false);
        singleton = this;
    }

    public override void OnClientExitRoom(){}


    private void OnEnable()
    {
        ButtonKick.onClick.AddListener(kickPlayer);
      
    }
    private void kickPlayer()
    {
        CmdKickPlayer(index);
    }

    // Ustala pozycje na ekranie na podstawie indeksu
    private void updatePositions()
    {
        roomManager = NetworkManager.singleton as NetworkRoomManagerExt;
        foreach (NetworkRoomPlayerExt item in roomManager.roomSlots)
        {
            Vector3 pos = getPlayerPosition(item.index);
            item.Panel.transform.position = pos;
        }
    }

    // Ustala czy przy graczu wyœwiatla siê opcja wyrzucenia z pokoju
    // Widoczne tylko dla admina tzn gracza z indeksem = 0
    private void updateButtonKick()
    {
        // TO DO
        //do poprawy
/*        if (index == 0)
        {
            ButtonKick.gameObject.SetActive(false);
        }
        else
        {
            room = NetworkManager.singleton as NetworkRoomManager;
            foreach (NetworkRoomPlayerExt item in room.roomSlots)
            {
                item.ButtonKick.gameObject.SetActive(false);
            }
        }*/

    }


    private Vector3 getPlayerPosition(int index)
    {
        switch (index)
        {
            case 0:
                return GameObject.FindGameObjectWithTag("PanelRoomPlayer0").transform.position;
            case 1:
                return GameObject.FindGameObjectWithTag("PanelRoomPlayer1").transform.position;
            case 2:
                return GameObject.FindGameObjectWithTag("PanelRoomPlayer2").transform.position;
            case 3:
                return GameObject.FindGameObjectWithTag("PanelRoomPlayer3").transform.position;
            default:
                return GameObject.FindGameObjectWithTag("PanelRoomPlayer0").transform.position;
        }
    }

    private Color32 getPlayerColor(int index)
    {
        switch(index)
        {
            case 0:
                return new Color32(0, 0, 255, 200);
            case 1:
                return new Color32(0, 255, 0, 200);
            case 2:
                return new Color32(255, 128, 0, 200);
            case 3:
                return new Color32(128, 0, 255, 200);
            case 4:
                return new Color32(255, 0, 0, 200);
            default:
                return new Color32(0, 0, 255, 200);
        }
    }
}
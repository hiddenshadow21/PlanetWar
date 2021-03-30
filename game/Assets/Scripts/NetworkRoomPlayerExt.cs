using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Mirror.Examples.NetworkRoom
{
    [AddComponentMenu("")]
    public class NetworkRoomPlayerExt : NetworkRoomPlayer
    {
        // UI Elements

        public Image ImagePlayerColor;
        public Text TextPlayerName;
        public Text TextReadyState;

        public Button ButtonKick;


        /// NetworkRoomPlayerExt singleton
        public static NetworkRoomPlayerExt singleton { get; private set; }

        static readonly ILogger logger = LogFactory.GetLogger(typeof(NetworkRoomPlayerExt));
        NetworkRoomManager room;


        [SyncVar(hook = nameof(PlayerNameChanged))]
        public string PlayerName;


        [SyncVar(hook = nameof(PlayerColorChanged))]
        public Color32 PlayerColor;

        #region Commands

        [Command]
        public void CmdChangePlayerName(string playerName)
        {
            room = NetworkManager.singleton as NetworkRoomManager;
            foreach (NetworkRoomPlayerExt item in room.roomSlots)
            {
                if (item == this)
                {
                    item.PlayerName = playerName;
                }
            }
        }

        [Command]
        public void CmdChangePlayerColor()
        {
            room = NetworkManager.singleton as NetworkRoomManager;
            foreach (NetworkRoomPlayerExt item in room.roomSlots)
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
        #endregion

        #region SyncVar Hooks

        private void PlayerNameChanged(string oldName, string newName)
        {
            Debug.Log("---Hook PlayerNameChanged---");
            Debug.Log(oldName);
            Debug.Log(newName);
            TextPlayerName.text = newName;
        }

        private void PlayerColorChanged(Color32 oldColor, Color32 newColor)
        {
            Debug.Log("---Hook PlayerNameColor---");
            ImagePlayerColor.color = newColor;
        }

        public override void IndexChanged(int oldIndex, int newIndex) 
        {
            Debug.Log("---IndexChanged---");
            Debug.Log(oldIndex);
            Debug.Log(newIndex);
            // chyba nie zadziala
            var panel = GameObject.FindGameObjectWithTag("PanelRoomPlayer");
            panel.transform.position = new Vector3(Screen.width / 2f, Screen.height - (newIndex * 160f) - 40f, 0);

            CmdChangePlayerColor();
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

        public override void OnStartClient()
        {
            Debug.Log("---OnStartClient()---");
            showRoomGUI = false;
            singleton = this;

            CmdChangePlayerName(PlayerInfo.Name);

            base.OnStartClient();
        }

        public override void OnClientEnterRoom()
        {
            Debug.Log("---OnClientEnterRoom()---");
        }

        public override void OnClientExitRoom(){}


        private void OnEnable()
        {
            ButtonKick.onClick.AddListener(kickPlayer);
      
        }
        private void kickPlayer()
        {
            // TO DO - FIX
            GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
        }

        private Color32 getPlayerColor(int index)
        {
            switch(index)
            {
                case 0:
                    return new Color32(255, 0, 0, 255);
                case 1:
                    return new Color32(255, 128, 0, 255);
                case 2:
                    return new Color32(255, 255, 0, 255);
                case 3:
                    return new Color32(0, 255, 0, 255);
                case 4:
                    return new Color32(0, 0, 255, 255);
                case 5:
                    return new Color32(255, 51, 153, 255);
                default:
                    return new Color32(0, 0, 0, 0);
            }
        }
    }
}
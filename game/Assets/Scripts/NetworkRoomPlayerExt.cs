using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mirror.Examples.NetworkRoom
{
    [AddComponentMenu("")]
    public class NetworkRoomPlayerExt : NetworkRoomPlayer
    {
        static readonly ILogger logger = LogFactory.GetLogger(typeof(NetworkRoomPlayerExt));

        [Tooltip("Name of the player.")]
        [SyncVar]
        public string playerName;

        public override void OnStartClient()
        {
            if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "OnStartClient {0}", SceneManager.GetActiveScene().path);

            name = PlayerInfo.Name;

            base.OnStartClient();
        }

        public override void OnClientEnterRoom()
        {
            if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "OnClientEnterRoom {0}", SceneManager.GetActiveScene().path);
        }

        public override void OnClientExitRoom()
        {
            if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "OnClientExitRoom {0}", SceneManager.GetActiveScene().path);
        }

        public override void ReadyStateChanged(bool _, bool newReadyState)
        {
            if (logger.LogEnabled()) logger.LogFormat(LogType.Log, "ReadyStateChanged {0}", newReadyState);
        }


        /// Render a UI for the room
        public override void OnGUI()
        {
            NetworkRoomManager room = NetworkManager.singleton as NetworkRoomManager;
            if (room)
            {
                if (!NetworkManager.IsSceneActive(room.RoomScene))
                    return;

                DrawPlayerReadyStateExt();
                DrawPlayerReadyButtonExt();
            }
        }

        // TO DO - Change view
        void DrawPlayerReadyStateExt()
        {
            GUILayout.BeginArea(new Rect(20f + (index * 100), 200f, 90f, 130f));

            GUILayout.Label(playerName);

            if (readyToBegin)
                GUILayout.Label("Ready");
            else
                GUILayout.Label("Not Ready");

            if (((isServer && index > 0) || isServerOnly) && GUILayout.Button("REMOVE"))
            {
                // This button only shows on the Host for all players other than the Host
                // Host and Players can't remove themselves (stop the client instead)
                // Host can kick a Player this way.
                GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
            }

            GUILayout.EndArea();
        }

        // TO DO - Change view
        void DrawPlayerReadyButtonExt()
        {
            if (NetworkClient.active && isLocalPlayer)
            {
                GUILayout.BeginArea(new Rect(20f, 300f, 120f, 20f));

                if (readyToBegin)
                {
                    if (GUILayout.Button("Cancel"))
                        CmdChangeReadyState(false);
                }
                else
                {
                    if (GUILayout.Button("Ready"))
                        CmdChangeReadyState(true);
                }

                GUILayout.EndArea();
            }

        }
    }
}
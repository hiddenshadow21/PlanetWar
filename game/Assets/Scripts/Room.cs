using Mirror.Examples.NetworkRoom;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Room : MonoBehaviour
{
    public Button ButtonReadyState;
    public Button ButtonExitRoom;
    //public Button ButtonStartGame;
    public Text TextRoomKey;

    NetworkRoomManagerExt roomManager;
    NetworkRoomPlayerExt roomPlayer;


    void Start()
    {
        changeShowGuiStatus(false);
        roomManager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<NetworkRoomManagerExt>();
        StartCoroutine(GetNetworkRoomPlayerExtSingleton(2));
        TextRoomKey.text += roomManager.Key;
    }

    IEnumerator GetNetworkRoomPlayerExtSingleton(float time)
    {
        yield return new WaitForSeconds(time);
        roomPlayer = NetworkRoomPlayerExt.singleton;
        changeShowGuiStatus(true);
    }

    private void changeShowGuiStatus(bool status)
    {
        ButtonReadyState.gameObject.SetActive(status);
        ButtonExitRoom.gameObject.SetActive(status);
        //ButtonStartGame.enabled = status;
    }

    private void OnEnable()
    {
        ButtonReadyState.onClick.AddListener(ChangeReadyState);
        ButtonExitRoom.onClick.AddListener(ExitRoom);
        //ButtonStartGame.onClick.AddListener(StartGame);
    }

    private void ChangeReadyState()
    {
        Debug.Log("networkRoomPlayerExt.readyToBegin = " + roomPlayer.readyToBegin);
        if (roomPlayer.readyToBegin)
        {
            ButtonReadyState.GetComponentInChildren<Text>().text = "Ready";  
        }
        else
        {
            ButtonReadyState.GetComponentInChildren<Text>().text = "Not ready";
        }
        roomPlayer.CmdChangeReadyState(!roomPlayer.readyToBegin);
    }

/*    private void StartGame()
    {
        int i = 0;

        foreach (var item in roomManager.roomSlots)
            if (item.readyToBegin)
                i++;
        if (i == roomManager.roomSlots.Count)
            roomManager.ServerChangeScene(roomManager.GameplayScene);
    }*/

    // TO DO - fix
    private void ExitRoom()
    {
        // TO DO
        // tu jesszcze usun polaczenie z serwerem
        roomManager.roomSlots.Remove(roomPlayer);
        Destroy(roomPlayer.gameObject);
        SceneManager.LoadScene(sceneName: "OfflineScene");
        roomManager.OnRoomServerDisconnect(roomPlayer.connectionToServer);
        Debug.Log($"connectionToServer: {roomPlayer.connectionToServer}");
        Debug.Log($"connectionToServer: {roomPlayer.connectionToClient}");
    }

}

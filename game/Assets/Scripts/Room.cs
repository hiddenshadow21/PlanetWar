using Mirror.Examples.NetworkRoom;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Room : MonoBehaviour
{
    public Button ButtonReadyState;
    public Button ButtonExitRoom;
    public Text TextRoomKey;

    NetworkRoomManagerExt roomManager;
    NetworkRoomPlayerExt roomPlayer;


    void Start()
    {
        changeShowGuiStatus(false);
        roomManager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<NetworkRoomManagerExt>();
        StartCoroutine(GetNetworkRoomPlayerExtSingleton(1));
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
    }

    private void OnEnable()
    {
        ButtonReadyState.onClick.AddListener(ChangeReadyState);
        ButtonExitRoom.onClick.AddListener(ExitRoom);
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

    private void ExitRoom()
    {
        roomPlayer.roomManager.StopClient();
    }

}

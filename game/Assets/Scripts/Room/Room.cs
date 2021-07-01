using Mirror.Examples.NetworkRoom;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;

public class Room : MonoBehaviour
{
    public Button ButtonReadyState;
    public Button ButtonExitRoom;
    public Text TextRoomKey;
    public GameObject AdminOptionsUI;
    public Dropdown DropdownMatchTime;

    NetworkRoomManagerExt roomManager;
    NetworkRoomPlayerExt roomPlayer;


    void Start()
    {
        changeShowGuiStatus(false);
        AdminOptionsUI.SetActive(false);
        StartCoroutine(GetNetworkRoomPlayerExtSingleton(1));
    }

    IEnumerator GetNetworkRoomPlayerExtSingleton(float time)
    {
        yield return new WaitForSeconds(time);
        roomManager = NetworkManager.singleton as NetworkRoomManagerExt;
        roomPlayer = NetworkRoomPlayerExt.singleton;
        TextRoomKey.text += roomManager.Key;
        if (roomPlayer.index == 0)
            AdminOptionsUI.SetActive(true);

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

    public void updateMatchTime(Dropdown dropdown)
    {
        switch (dropdown.value)
        {
            case 0:
                roomPlayer.CmdChangeMatchTime(180);
                break;
            case 1:
                roomPlayer.CmdChangeMatchTime(300);
                break;
            case 2:
                roomPlayer.CmdChangeMatchTime(600);
                break;
            default:
                roomPlayer.CmdChangeMatchTime(180);
                break;
        }

        var nM = NetworkManager.singleton as NetworkRoomManagerExt;
    }

}

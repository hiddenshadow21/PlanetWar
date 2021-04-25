using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    //CHAT//
    public event EventHandler<string> ChatMessageEntered;
    public event EventHandler<bool> ChatStatusChanged;
    public Text[] Chats;
    public Text ChatInfo;
    public InputField ChatMessage;
    //CHAT//

    //DEATH//
    public GameObject DeathImage;
    public Text EnemyKilled;
    public Text EnemyKilledNumber;
    public Text DeathNumber;
    //DEATH//

    //AMMO//
    public GameObject ReloadAlert;
    public GameObject ReloadingAlert;
    public Text ReloadInfo;
    public Text Ammo;
    //AMMO//

    //HEALTH//
    public Text HP;
    //HEALTH//

    //TIMER//
    public Text Timer;
    //TIMER//

    private readonly GUIStyle style = new GUIStyle();

    private void Awake()
    {
        foreach (Text c in Chats)
        {
            c.text = "";
        }
        ChatInfo.gameObject.SetActive(true);
        ChatMessage.gameObject.SetActive(false);
        ReloadAlert.gameObject.SetActive(false);
        ReloadingAlert.gameObject.SetActive(false);
        ReloadInfo.gameObject.SetActive(false);
        EnemyKilled.gameObject.SetActive(false);
        DeathImage.SetActive(false);
        style.richText = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (ChatMessage.gameObject.active == true)
            {
                ChatStatusChanged?.Invoke(this, false);
                ChatMessage.gameObject.SetActive(false);
                ChatInfo.gameObject.SetActive(true);
            }
            else
            {
                ChatStatusChanged?.Invoke(this, true);
                ChatMessage.gameObject.SetActive(true);
                ChatMessage.ActivateInputField();
                ChatInfo.gameObject.SetActive(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (ChatMessage.gameObject.active == true)
            {
                ChatStatusChanged?.Invoke(this, false);
                ChatMessageEntered?.Invoke(this, ChatMessage.text);
                ChatMessage.gameObject.SetActive(false);
                ChatInfo.gameObject.SetActive(true);
                ChatMessage.text = "";
            }
        }
    }

    public void UpdateHealth(int hp)
    {
        if(hp <= 20)
        {
            HP.color = new Color(1, 0, 0);
        }
        else
        {
            HP.color = new Color(0.36f, 0.92f, 0.39f);
        }
        HP.text = hp.ToString();
    }

    public void UpdateAmmo(int currAmmo, int maxAmmo)
    {
        if(currAmmo == 0)
        {
            Ammo.text = "<color=#ff0000>" + currAmmo + "</color>/" + maxAmmo;
        }
        else
        {
            Ammo.text = currAmmo + "/" + maxAmmo;
        }
    }

    public void UpdateTimer(int secs)
    {
        decimal m = (int)Math.Floor(secs / 60d);
        int s = (int)(secs - m * 60);

        string tmpTime = m.ToString() + ":";
        if (s < 10)
            tmpTime += "0";
        tmpTime += s;

        Timer.text = tmpTime;
    }

    public void UpdateEnemyKilledNumber(uint enemyKilledNumber)
    {
        EnemyKilledNumber.text = enemyKilledNumber.ToString();
    }

    public void UpdateDeathNumber(uint deathNumber)
    {
        DeathNumber.text = deathNumber.ToString();
    }

    public void ShowDeathInfo(string shooter, string killed)
    {
        CancelInvoke(nameof(hideDeathInfo));
        EnemyKilled.gameObject.SetActive(true);
        DeathImage.SetActive(true);
        EnemyKilled.text = "<size=40><color=#69e5fe>DEFEATED</color></size>\n<color=#d4354a>" + killed + "</color> <size=15><color=#ffffff>killed by</color></size> <color=#5DEC64>" + shooter + "</color>";
        Invoke(nameof(hideDeathInfo), 5f);
    }

    public void SetNewChatMessage(string username, string message, bool itIsMyMessage = false)
    {
        if(message != "")
        {
            CancelInvoke(nameof(updateChatPosition));
            updateChatPosition();
            if (itIsMyMessage == true)
            {
                Chats[0].text = "<size=25><color=#ffffff>" + username + "</color></size><size=20><color=#69e5fe>: " + message + "</color></size>";
            }
            else
            {
                Chats[0].text = "<size=25><color=#69e5fe>" + username + "</color></size><size=20><color=#ffffff>: " + message + "</color></size>";
            }
        }
    }

    public void ShowEmptyAmmoInfo()
    {
        CancelInvoke(nameof(hideEmptyAmmoInfo));
        ReloadAlert.gameObject.SetActive(true);
        ReloadInfo.gameObject.SetActive(true);
        ReloadInfo.text = "<color=#69e5fe>R</color> reload";
        Invoke(nameof(hideEmptyAmmoInfo), 1.5f);
    }

    public void ShowReloadingAmmoInfo(float reloadSpeed)
    {
        ReloadingAlert.gameObject.SetActive(true);
        ReloadAlert.gameObject.SetActive(false);
        ReloadInfo.text = "<color=#ffffff>Reloading</color>";
        ReloadInfo.gameObject.SetActive(true);
        Invoke(nameof(HideReloadingAmmoInfo), reloadSpeed);
    }

    public void HideReloadingAmmoInfo()
    {
        ReloadingAlert.gameObject.SetActive(false);
        ReloadInfo.gameObject.SetActive(false);
    }

    private void updateChatPosition()
    {
        for (int i = Chats.Length - 1; i > 0; i--)
        {
            Chats[i].text = Chats[i - 1].text;
        }

        Chats[0].text = "";
        Invoke(nameof(updateChatPosition), 15.0f);
    }

    private void hideDeathInfo()
    {
        EnemyKilled.gameObject.SetActive(false);
        DeathImage.SetActive(false);
    }

    private void hideEmptyAmmoInfo()
    {
        ReloadAlert.gameObject.SetActive(false);
        ReloadInfo.gameObject.SetActive(false);
    }
}

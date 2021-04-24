using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public event EventHandler<string> ChatMessageEntered;
    public event EventHandler<bool> ChatStatusChanged;
    public GameObject DeathImage;
    public Text Health;
    public Text Ammo;
    public Text Timer;
    public Text EnemyKilled;
    public Text EnemyKilledNumber;
    public Text[] Chats;
    public Text Info;
    public InputField Message;
    private readonly GUIStyle style = new GUIStyle();

    private void Awake()
    {
        foreach (Text c in Chats)
        {
            c.text = "";
        }
        Info.gameObject.SetActive(true);
        Message.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Message.gameObject.active == true)
            {
                ChatStatusChanged?.Invoke(this, false);
                Message.gameObject.SetActive(false);
                Info.gameObject.SetActive(true);
            }
            else
            {
                ChatStatusChanged?.Invoke(this, true);
                Message.gameObject.SetActive(true);
                Message.ActivateInputField();
                Info.gameObject.SetActive(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (Message.gameObject.active == true)
            {
                ChatStatusChanged?.Invoke(this, false);
                ChatMessageEntered?.Invoke(this, Message.text);
                Message.gameObject.SetActive(false);
                Info.gameObject.SetActive(true);
                Message.text = "";
            }
        }
    }

    private void Start()
    {
        EnemyKilled.gameObject.SetActive(false);
        DeathImage.SetActive(false);
        style.richText = true;
    }

    public void UpdateHealth(int hp)
    {
        if(hp <= 20)
        {
            Health.color = new Color(1, 0, 0);
        }
        else
        {
            Health.color = new Color(0.36f, 0.92f, 0.39f);
        }
        Health.text = hp.ToString();
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
        int m = secs % 60;
        int s = m * 60 - secs;

        Timer.text = m + ":" + s;
    }

    public void UpdateEnemyKilledNumber(uint enemyKilledNumber)
    {
        EnemyKilledNumber.text = enemyKilledNumber.ToString();
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
}

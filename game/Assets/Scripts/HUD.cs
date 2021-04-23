using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public event EventHandler<string> OnChatMessageEntered;
    public GameObject DeathImage;
    public Text Health;
    public Text Ammo;
    public Text Timer;
    public Text EnemyKilled;
    public Text EnemyKilledNumber;
    public Text Chat1;
    public Text Chat2;
    public Text Chat3;
    public Text Chat4;
    public Text Chat5;
    public Text Chat6;
    public Text Chat7;
    public Text Chat8;
    public Text Info;
    public InputField Message;
    private readonly List<Text> chats = new List<Text>();
    private readonly GUIStyle style = new GUIStyle();

    private void Awake()
    {
        chats.Add(Chat1);
        chats.Add(Chat2);
        chats.Add(Chat3);
        chats.Add(Chat4);
        chats.Add(Chat5);
        chats.Add(Chat6);
        chats.Add(Chat7);
        chats.Add(Chat8);
        foreach (Text c in chats)
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
                Message.gameObject.SetActive(false);
                Info.gameObject.SetActive(true);
            }
            else
            {
                Message.gameObject.SetActive(true);
                Message.ActivateInputField();
                Info.gameObject.SetActive(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (Message.gameObject.active == true)
            {
                OnChatMessageEntered?.Invoke(this, Message.text);
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
                chats[0].text = "<size=25><color=#ffffff>" + username + "</color></size><size=20><color=#69e5fe>: " + message + "</color></size>";
            }
            else
            {
                chats[0].text = "<size=25><color=#69e5fe>" + username + "</color></size><size=20><color=#ffffff>: " + message + "</color></size>";
            }
        }
    }

    private void updateChatPosition()
    {
        for (int i = chats.Count - 1; i > 0; i--)
        {
            chats[i].text = chats[i - 1].text;
        }

        chats[0].text = "";
        Invoke(nameof(updateChatPosition), 15.0f);
    }

    private void hideDeathInfo()
    {
        EnemyKilled.gameObject.SetActive(false);
        DeathImage.SetActive(false);
    }
}

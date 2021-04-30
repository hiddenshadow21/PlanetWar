using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    private readonly GUIStyle style = new GUIStyle();

    #region Chat
    public event EventHandler<string> Chat_messageEntered;
    public Text[] Chat_chats;
    public Text Chat_info;
    public InputField Chat_message;

    public bool Chat_isChatActive
    { 
        get
        {
            return Chat_message.gameObject.activeSelf;
        }
    }

    private void chat_init()
    {
        foreach (Text c in Chat_chats)
        {
            c.text = "";
        }

        Chat_info.gameObject.SetActive(true);
        Chat_message.gameObject.SetActive(false);
    }

    public void Chat_TabAction()
    {
        if (Chat_message.gameObject.activeSelf == true)
        {
            Chat_message.gameObject.SetActive(false);
            Chat_info.gameObject.SetActive(true);
        }
        else
        {
            Chat_message.gameObject.SetActive(true);
            Chat_message.ActivateInputField();
            Chat_info.gameObject.SetActive(false);
        }
    }

    public void Chat_ReturnAction()
    {
        if (Chat_message.gameObject.activeSelf == true)
        {
            Chat_messageEntered?.Invoke(this, Chat_message.text);
            Chat_message.gameObject.SetActive(false);
            Chat_info.gameObject.SetActive(true);
            Chat_message.text = "";
        }
    }

    public void Chat_update(List<string> chats, string playerName)
    {
        for(int i = 0; i < chats.Count; i++)
        {
            string[] data = chats[i].Split('~');
            if(data.Length == 2)
            {
                if (data[0] == playerName)
                {
                    Chat_chats[i].text = "<size=25><color=#ffffff>" + data[0] + "</color></size><size=20><color=#69e5fe>: " + data[1] + "</color></size>";
                }
                else
                {
                    Chat_chats[i].text = "<size=25><color=#69e5fe>" + data[0] + "</color></size><size=20><color=#ffffff>: " + data[1] + "</color></size>";
                }
            }
            else
            {
                Chat_chats[i].text = "";
            }
        }
    }
    #endregion

    #region Timer
    public Text Timer_timer;

    public void Timer_update(int secs)
    {
        decimal m = (int)Math.Floor(secs / 60d);
        int s = (int)(secs - m * 60);
        string tmpTime = m.ToString() + ":";

        if (s < 10)
            tmpTime += "0";

        tmpTime += s;
        Timer_timer.text = tmpTime;
    }
    #endregion

    #region HP
    public Text HP_hp;
    public Text HP_respawningInfo;
    public GameObject HP_respawningImage;

    public void HP_update(int hp)
    {
        if (hp <= 20)
        {
            HP_hp.color = new Color(1, 0, 0);
        }
        else
        {
            HP_hp.color = new Color(0.36f, 0.92f, 0.39f);
        }

        HP_hp.text = hp.ToString();
    }

    public IEnumerator HP_showRespawnAnim(float seconds, float secDivision = 2)
    {
        float hpTickSize = 100 / (seconds * secDivision);
        float hp = 0;

        HP_respawningInfo.gameObject.SetActive(true);
        HP_respawningImage.SetActive(true);

        while (hp < 100)
        {
            hp += hpTickSize;
            HP_update((int)Math.Round(hp));
            seconds -= seconds / secDivision;
            yield return new WaitForSeconds(1/secDivision);
        }

        HP_respawningInfo.gameObject.SetActive(false);
        HP_respawningImage.SetActive(false);
    }

    private void hp_init()
    {
        HP_respawningInfo.gameObject.SetActive(false);
        HP_respawningImage.SetActive(false);
    }
    #endregion

    #region Ammo
    public GameObject Ammo_reloadImage;
    public GameObject Ammo_reloadingImage;
    public Text Ammo_reloadReloadingInfo;
    public Text Ammo_ammo;

    public void Ammo_update(int currAmmo, int maxAmmo)
    {
        if (currAmmo == 0)
        {
            Ammo_ammo.text = "<color=#ff0000>" + currAmmo + "</color>/" + maxAmmo;
        }
        else
        {
            Ammo_ammo.text = currAmmo + "/" + maxAmmo;
        }
    }

    public void Ammo_showEmptyAmmoInfo()
    {
        CancelInvoke(nameof(ammo_hideEmptyAmmoInfo));
        Ammo_reloadImage.gameObject.SetActive(true);
        Ammo_reloadReloadingInfo.gameObject.SetActive(true);
        Ammo_reloadReloadingInfo.text = "<color=#69e5fe>R</color> reload";
        Invoke(nameof(ammo_hideEmptyAmmoInfo), 1.5f);
    }

    public void Ammo_showReloadingAmmoInfo(float reloadSpeed)
    {
        ammo_hideEmptyAmmoInfo();
        CancelInvoke(nameof(ammo_hideReloadingAmmoInfo));
        CancelInvoke(nameof(ammo_hideEmptyAmmoInfo));
        Ammo_reloadingImage.gameObject.SetActive(true);
        Ammo_reloadReloadingInfo.gameObject.SetActive(true);
        Ammo_reloadReloadingInfo.text = "<color=#69e5fe>Reloading</color>";
        Invoke(nameof(ammo_hideReloadingAmmoInfo), reloadSpeed);
    }

    private void ammo_init()
    {
        Ammo_reloadImage.gameObject.SetActive(false);
        Ammo_reloadingImage.gameObject.SetActive(false);
        Ammo_reloadReloadingInfo.gameObject.SetActive(false);
    }

    private void ammo_hideReloadingAmmoInfo()
    {
        Ammo_reloadingImage.gameObject.SetActive(false);
        Ammo_reloadReloadingInfo.gameObject.SetActive(false);
    }

    private void ammo_hideEmptyAmmoInfo()
    {
        Ammo_reloadImage.gameObject.SetActive(false);
        Ammo_reloadReloadingInfo.gameObject.SetActive(false);
    }
    #endregion

    #region Kills
    public Text Kills_kills;
    public Text Kills_increment;
    public GameObject Kills_incrementImage;

    public void Kills_update(uint enemyKilledNumber)
    {
        Kills_kills.text = enemyKilledNumber.ToString();

        if (enemyKilledNumber != 0)
        {
            CancelInvoke(nameof(kills_hideIncrementInfo));
            Kills_increment.gameObject.SetActive(true);
            Kills_incrementImage.SetActive(true);
            Invoke(nameof(kills_hideIncrementInfo), 5f);
        }
    }

    private void kills_hideIncrementInfo()
    {
        Kills_increment.gameObject.SetActive(false);
        Kills_incrementImage.SetActive(false);
    }

    private void kills_init()
    {
        Kills_increment.gameObject.SetActive(false);
        Kills_incrementImage.SetActive(false);
    }

    #endregion

    #region Deaths
    public Text Deaths_deaths;
    public Text Deaths_increment;
    public GameObject Deaths_incrementImage;

    public void Deahts_update(uint deathNumber)
    {
        Deaths_deaths.text = deathNumber.ToString();

        if(deathNumber != 0)
        {
            CancelInvoke(nameof(deaths_hideIncrementInfo));
            Deaths_increment.gameObject.SetActive(true);
            Deaths_incrementImage.SetActive(true);
            Invoke(nameof(deaths_hideIncrementInfo), 5f);
        }
    }

    private void deaths_hideIncrementInfo()
    {
        Deaths_increment.gameObject.SetActive(false);
        Deaths_incrementImage.SetActive(false);
    }

    private void deaths_init()
    {
        Deaths_increment.gameObject.SetActive(false);
        Deaths_incrementImage.SetActive(false);
    }
    #endregion

    #region DeathGlobal
    public GameObject DeathGlobal_image;
    public Text DeathGlobal_text;

    public void DeathGlobal_show(string shooter, string killed)
    {
        CancelInvoke(nameof(deathGlobal_hide));
        DeathGlobal_text.gameObject.SetActive(true);
        DeathGlobal_image.SetActive(true);
        DeathGlobal_text.text = "<size=30><color=#69e5fe>DEFEATED</color></size>\n<color=#d4354a>" + killed + "</color><size=15><color=#ffffff> killed by </color></size><color=#5DEC64>" + shooter + "</color>";
        Invoke(nameof(deathGlobal_hide), 5f);
    }

    private void deathGlobal_hide()
    {
        DeathGlobal_text.gameObject.SetActive(false);
        DeathGlobal_image.SetActive(false);
    }

    private void deathGlobal_init()
    {
        DeathGlobal_text.gameObject.SetActive(false);
        DeathGlobal_image.SetActive(false);
    }
    #endregion

    private void Awake()
    {
        chat_init();
        ammo_init();
        kills_init();
        deaths_init();
        hp_init();
        deathGlobal_init();
        style.richText = true;
    }
}

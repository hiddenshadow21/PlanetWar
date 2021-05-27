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
        for (int i = 0; i < chats.Count; i++)
        {
            string[] data = chats[i].Split('~');
            if (data.Length == 2)
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
        if(secs < 10)
        {
            Timer_timer.color = new Color(1, 0, 0);
        }
        Timer_timer.text = tmpTime;
    }
    #endregion

    #region HP
    public Text HP_hp;
    public Text HP_info;
    public GameObject HP_infoImage;

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

        HP_info.gameObject.SetActive(true);
        HP_info.text = "RESPAWNING";
        HP_infoImage.SetActive(true);

        while (hp < 100)
        {
            hp += hpTickSize;
            HP_update((int)Math.Round(hp));
            seconds -= seconds / secDivision;
            yield return new WaitForSeconds(1 / secDivision);
        }

        HP_info.gameObject.SetActive(false);
        HP_infoImage.SetActive(false);
    }

    public IEnumerator HP_showHpIncrementAnim(float incrementHp, float seconds = 3)
    {
        HP_info.gameObject.SetActive(true);
        HP_info.text = "+" + incrementHp.ToString();
        HP_infoImage.SetActive(true);
        yield return new WaitForSeconds(seconds);

        HP_info.gameObject.SetActive(false);
        HP_infoImage.SetActive(false);
    }

    private void hp_init()
    {
        HP_info.gameObject.SetActive(false);
        HP_infoImage.SetActive(false);
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

        if (deathNumber != 0)
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

    #region Death [global messages section]
    public GameObject Death_image;
    public Text Death_text;

    public void Death_show(string shooter, string killed)
    {
        hideGlobalMessagesSection();
        Death_text.gameObject.SetActive(true);
        Death_image.SetActive(true);
        Death_text.text = "<size=30><color=#69e5fe>DEFEATED</color></size>\n<color=#d4354a>" + killed + "</color><size=15><color=#ffffff> killed by </color></size><color=#5DEC64>" + shooter + "</color>";
        Invoke(nameof(death_hide), 5f);
    }

    private void death_hide()
    {
        Death_text.gameObject.SetActive(false);
        Death_image.SetActive(false);
    }

    private void death_init()
    {
        Death_text.gameObject.SetActive(false);
        Death_image.SetActive(false);
    }
    #endregion

    #region Armor
    public Text Armor_armor;
    public Text Armor_info;
    public GameObject Armor_infoImage;

    public void Armor_update(int armor)
    {
        Armor_armor.text = armor.ToString();
    }

    public IEnumerator Armor_showArmorIncrementAnim(float incrementArmor, float seconds = 3)
    {
        Armor_info.gameObject.SetActive(true);
        Armor_info.text = "+" + incrementArmor.ToString();
        Armor_infoImage.SetActive(true);
        yield return new WaitForSeconds(seconds);

        Armor_infoImage.gameObject.SetActive(false);
        Armor_info.gameObject.SetActive(false);
    }

    private void armor_init()
    {
        Armor_info.gameObject.SetActive(false);
        Armor_infoImage.SetActive(false);
    }
    #endregion

    #region Bonus - GunSpeedChanger
    public Text Bonus_gunSpeedChanger_text;
    public GameObject Bonus_gunSpeedChanger_image;

    public IEnumerator Bonus_gunSpeedChanger_show(float seconds)
    {
        Bonus_gunSpeedChanger_text.gameObject.SetActive(true);
        Bonus_gunSpeedChanger_image.SetActive(true);

        while (seconds != 0)
        { 
            Bonus_gunSpeedChanger_text.text = seconds.ToString();
            seconds--;
            yield return new WaitForSeconds(1);
        }

        Bonus_gunSpeedChanger_text.gameObject.SetActive(false);
        Bonus_gunSpeedChanger_image.SetActive(false);
    }

    private void bonus_gunSpeedChanger_init()
    {
        Bonus_gunSpeedChanger_text.gameObject.SetActive(false);
        Bonus_gunSpeedChanger_image.SetActive(false);
    }
    #endregion

    #region Bonus - CarpetBombing [global messages section]
    public GameObject Bonus_carpetBombing_image;
    public Text Bonus_carpetBombing_text;

    public void Bonus_carpetBombing_show(string summonerName)
    {
        hideGlobalMessagesSection();
        Bonus_carpetBombing_text.gameObject.SetActive(true);
        Bonus_carpetBombing_image.SetActive(true);
        Bonus_carpetBombing_text.text = "<size=30><color=#69e5fe>CARPET BOMBING</color></size>\n<size=15><color=#ffffff> summoned by </color></size><color=#5DEC64>" + summonerName + "</color>";
        Invoke(nameof(bonus_carpetBombing_hide), 3f);
    }

    private void bonus_carpetBombing_hide()
    {
        Bonus_carpetBombing_text.gameObject.SetActive(false);
        Bonus_carpetBombing_image.SetActive(false);
    }

    private void bonus_carpetBombing_init()
    {
        Bonus_carpetBombing_text.gameObject.SetActive(false);
        Bonus_carpetBombing_image.SetActive(false);
    }
    #endregion

    #region Damage
    public GameObject Damage_image;
    private Coroutine coroutine;

    public void Damage_show(float damage)
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(damage_show(damage));
    }

    public IEnumerator damage_show(float damage)
    {
        Damage_image.SetActive(true);

        while (damage >= 0)
        {
            Damage_image.GetComponent<SpriteRenderer>().material.SetColor("_Color", new Color(1, 1, 1, 5.6f * damage / 255f));
            damage--;
            yield return new WaitForSeconds(0.025f);
        }

        Damage_image.SetActive(false);
    }

    private void damage_init()
    {
        Damage_image.SetActive(false);
    }
    #endregion

    #region PoisonArea
    public Text PoisonArea_text;

    public void PoisonArea_SetValue(int poisonedAreaNumber, int allAreasNumber)
    {
        PoisonArea_text.text = poisonedAreaNumber.ToString() + "/" + allAreasNumber.ToString();
    }

    private void poisonArea_init()
    {
        PoisonArea_text.gameObject.SetActive(true);
        PoisonArea_text.text = "0/0";
    }
    #endregion

    #region Bonus - PoisonAreaShield
    public Text Bonus_poisonAreaShield_text;
    public GameObject Bonus_poisonAreaShield_image;

    public IEnumerator Bonus_poisonAreaShield_show(float seconds)
    {
        Bonus_poisonAreaShield_text.gameObject.SetActive(true);
        Bonus_poisonAreaShield_image.SetActive(true);

        while (seconds != 0)
        {
            Bonus_poisonAreaShield_text.text = seconds.ToString();
            seconds--;
            yield return new WaitForSeconds(1);
        }

        Bonus_poisonAreaShield_text.gameObject.SetActive(false);
        Bonus_poisonAreaShield_image.SetActive(false);
    }

    private void bonus_poisonAreaShield_init()
    {
        Bonus_poisonAreaShield_text.gameObject.SetActive(false);
        Bonus_poisonAreaShield_image.SetActive(false);
    }
    #endregion

    #region Bonus - CarpetBombingShield
    public Text Bonus_carpetBombingShield_text;
    public GameObject Bonus_carpetBombingShield_image;

    public IEnumerator Bonus_carpetBombingShield_show(float seconds)
    {
        Bonus_carpetBombingShield_text.gameObject.SetActive(true);
        Bonus_carpetBombingShield_image.SetActive(true);

        while (seconds != 0)
        {
            Bonus_carpetBombingShield_text.text = seconds.ToString();
            seconds--;
            yield return new WaitForSeconds(1);
        }

        Bonus_carpetBombingShield_text.gameObject.SetActive(false);
        Bonus_carpetBombingShield_image.SetActive(false);
    }

    private void bonus_carpetBombingShield_init()
    {
        Bonus_carpetBombingShield_text.gameObject.SetActive(false);
        Bonus_carpetBombingShield_image.SetActive(false);
    }
    #endregion

    private void hideGlobalMessagesSection() //call this method for elements with [global messages section] tag
    {
        CancelInvoke(nameof(death_hide));
        death_hide();

        CancelInvoke(nameof(bonus_carpetBombing_hide));
        bonus_carpetBombing_hide();
    }

    private void Awake()
    {
        chat_init();
        ammo_init();
        kills_init();
        deaths_init();
        hp_init();
        death_init();
        armor_init();
        bonus_gunSpeedChanger_init();
        bonus_carpetBombing_init();
        damage_init();
        poisonArea_init();
        bonus_carpetBombingShield_init();
        bonus_poisonAreaShield_init();
        style.richText = true;
    }
}

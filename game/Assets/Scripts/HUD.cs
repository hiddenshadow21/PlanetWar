using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public GameObject DeathImage;
    public Text Health;
    public Text Ammo;
    public Text Timer;
    public Text EnemyKilled;
    public Text EnemyKilledNumber;
    private readonly GUIStyle style = new GUIStyle();

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

    public void UpdateAmmo(int maxAmmo, int currAmmo)
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

    public void UpdateEnemyKilledNumber(int enemyKilledNumber)
    {
        EnemyKilledNumber.text = enemyKilledNumber.ToString();
    }

    public void ShowDeathInfo(string shooter, string killed)
    {
        CancelInvoke("hideDeathInfo");
        EnemyKilled.gameObject.SetActive(true);
        DeathImage.SetActive(true);
        EnemyKilled.text = "<size=40><color=#69e5fe>DEFEATED</color></size>\n<color=#d4354a>" + killed + "</color> <size=15><color=#ffffff>killed by</color></size> <color=#5DEC64>" + shooter + "</color>";
        Invoke("hideDeathInfo", 5f);
    }

    private void hideDeathInfo()
    {
        EnemyKilled.gameObject.SetActive(false);
        DeathImage.SetActive(false);
    }
}

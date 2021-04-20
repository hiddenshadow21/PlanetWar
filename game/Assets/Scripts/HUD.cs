using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Text Health;
    public Text Ammo;
    public Text Timer;

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
        Ammo.text = currAmmo.ToString() + "/" + maxAmmo.ToString();
    }

    public void UpdateTimer(int secs)
    {
        int m = secs % 60;
        int s = m * 60 - secs;

        Timer.text = m + ":" + s;
    }
}

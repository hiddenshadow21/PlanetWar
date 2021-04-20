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
        Health.text = hp.ToString();
        if(hp < 20)
        {
            Health.color = new Color(255, 0, 0);
        }
        else
        {
            Health.color = new Color(93, 236, 100, 255);
        }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public static bool OptionsIsOff = true;

    public GameObject pauseMenuUI;

    void Update()
    {
   
            if (Input.GetKeyDown(KeyCode.Escape) && OptionsIsOff)
            {

                if (GameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
  

    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        OptionsIsOff = true;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Options()
    {
        OptionsIsOff = false;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        OptionsIsOff = true;
        GameIsPaused = false;
        SceneManager.LoadScene("Main");
    }

    public void QuitGame()
    {
        Debug.Log("You have quit the game");
        Application.Quit();
    }
}

using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Log : MonoBehaviour
{
    public InputField Email; 
    public InputField Password; 
    public Button Login;
    public Text Answer;
    public Button ToEmailCheck;
    public Button ToReg;
    public Button Exit;

    private const string loginURL = "http://40.69.215.163/logreg/login.php";
    private const char fieldSeparator = ':';
    
    private void callLogin()
    {
        Login.interactable = false;
        StartCoroutine(login());
    }

    private void Start()
    {
        Email.Select();
        ValuesTransfer.Email = "";
        ValuesTransfer.Code = "";
    }

    private void OnEnable()
    {
        Login.onClick.AddListener(callLogin);
        ToReg.onClick.AddListener(toReg);
        ToEmailCheck.onClick.AddListener(toEmailCheck);
        Exit.onClick.AddListener(QuitGame);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            switch (EventSystem.current.currentSelectedGameObject.name)
            {
                case "Email_input":
                    Password.Select();
                    break;
                case "Password_input":
                    if(Login.interactable == true)
                    {
                        Login.Select();
                    }
                    else
                    {
                        ToEmailCheck.Select();
                    }
                    break;
                case "Login_button":
                    ToEmailCheck.Select();
                    break;
                case "ToEmailCheck_button":
                    ToReg.Select();
                    break;
                case "ToReg_button":
                    Exit.Select();
                    break;
                case "Exit_button":
                    Email.Select();
                    break;
            }
        }

        Login.interactable = (Email.text != "" && Password.text != "");

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(Login.interactable == true)
            {
                callLogin();
            }
        }
    }

    private bool passwordTest(string serverData, string passwd)
    {
        byte[] mergedData = Convert.FromBase64String(serverData);
        byte[] salt = new byte[16];

        Array.Copy(mergedData, 0, salt, 0, 16);
        var rfc2989db = new Rfc2898DeriveBytes(passwd, salt, 10000);
        byte[] hash = rfc2989db.GetBytes(20);

        for (int i = 0; i < 20; i++)
        {
            if (hash[i] != mergedData[i + 16])
            {
                return false;
            }
        }
        return true;
    }

    private IEnumerator login()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", Email.text);

        UnityWebRequest www = UnityWebRequest.Post(loginURL, form);
        yield return www.SendWebRequest();

        outputInterpreter(www.downloadHandler.text);
    }

    private void outputInterpreter(string serverResponse)
    {
        string[] response = serverResponse.Split(fieldSeparator);

        switch (response[0])
        {
            case "L_AU_SUCCESSFUL":
                try
                {
                    if(passwordTest(response[1], Password.text) == true)
                    {
                        PlayerInfo.Id = int.Parse(response[2]);
                        PlayerInfo.Name = response[3];
                        PlayerInfo.EMail = response[4];
                        Answer.text = "Login successful!";
                        SceneManager.LoadScene(sceneName: "OfflineScene");
                    }
                    else
                    {
                        goto case "L_AU_1";
                    }
                }
                catch (IndexOutOfRangeException e)
                {
                    Answer.text = "Error (Code: L_OI_1)! " + e.Message;
                }
                break;
            case "L_AU_1":
            case "L_AU_2":
                Answer.text = "Incorrect login or password!";
                break;
            case "L_AU_4":
                Answer.text = "This account is not verified!";
                break;
            default:
                if(response[0] == "")
                {
                    Answer.text = "Error! Please check Your internet connection and try again later!";
                }
                else
                {
                    Answer.text = "Error (Code: " + response[0] + ")! Please try again later!";
                }
                break;
        }
        Login.interactable = true;
    }

    private void toReg()
    {
        SceneManager.LoadScene(sceneName: "Reg");
    }

    private void toEmailCheck()
    {
        SceneManager.LoadScene(sceneName: "EmailCheck");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
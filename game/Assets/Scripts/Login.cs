using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public InputField Email; //0
    public InputField Password; //1
    public Button Send; //2
    public Button ToRegistration; //3
    public Text Answer;

    private const string loginURL = "http://40.69.215.163/login.php";
    private const char fieldSeparator = ':';
    
    private int activeSwitchID;

    private void callLogin()
    {
        StartCoroutine(login());
    }

    private void Start()
    {
        Email.Select();
        activeSwitchID = 0;
    }

    private void OnEnable()
    {
        Send.onClick.AddListener(callLogin);
        ToRegistration.onClick.AddListener(toRegistration);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            switch (activeSwitchID)
            {
                case 0:
                    Password.Select();
                    activeSwitchID = 1;
                    break;
                case 1:
                    if(Send.interactable == true)
                    {
                        activeSwitchID = 2;
                        Send.Select();
                    }
                    else
                    {
                        activeSwitchID = 3;
                        ToRegistration.Select();
                    }
                    break;
                case 2:
                    activeSwitchID = 3;
                    ToRegistration.Select();
                    break;
                case 3:
                    activeSwitchID = 0;
                    Email.Select();
                    break;
            }
        }

        Send.interactable = (Email.text != "" && Password.text != "");

        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(Send.interactable == true)
            {
                callLogin();
            }
        }
    }

    private IEnumerator login()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", Email.text);
        form.AddField("password", Password.text);

        UnityWebRequest www = UnityWebRequest.Post(loginURL, form);
        yield return www.SendWebRequest();

        string[] dbResponse = www.downloadHandler.text.Split(fieldSeparator);

        switch (dbResponse[0])
        {
            case "LS_1":
                try
                {
                    PlayerInfo.Id = int.Parse(dbResponse[1]);
                    PlayerInfo.Name = dbResponse[2];
                    PlayerInfo.EMail = dbResponse[3];
                    Answer.text = "Login successful!";
                    //tutaj przejście do kolejnej sceny!
                }
                catch (IndexOutOfRangeException e)
                {
                    Answer.text = "Error (Code: CLIENT_DB_1)! " + e.Message;
                }
                break;
            case "L_1":
            case "L_2":
                Answer.text = "Incorrect login or password!";
                break;
            default:
                Answer.text = "Error (Code: " + www.downloadHandler.text + ")! Please try again later!";
                break;
        }
    }

    private void toRegistration()
    {
        SceneManager.LoadScene(sceneName: "Reg");
    }
}
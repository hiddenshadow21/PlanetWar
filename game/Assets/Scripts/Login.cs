using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public InputField Email; 
    public InputField Password; 
    public Button Send; 
    public Button ToRegistration; 
    public Text Answer;

    private const string loginURL = "http://40.69.215.163/logreg/login.php";
    private const char fieldSeparator = ':';
    

    private void callLogin()
    {
        StartCoroutine(login());
    }

    private void Start()
    {
        Email.Select();
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
            switch (EventSystem.current.currentSelectedGameObject.name)
            {
                case "Email_input":
                    Password.Select();
                    break;
                case "Password_input":
                    if(Send.interactable == true)
                    {
                        Send.Select();
                    }
                    else
                    {
                        ToRegistration.Select();
                    }
                    break;
                case "Login_button":
                    ToRegistration.Select();
                    break;
                case "ToRegistration_button":
                    Email.Select();
                    break;
            }
        }

        Send.interactable = (Email.text != "" && Password.text != "");

        if (Input.GetKeyDown(KeyCode.Return))
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
                    PlayerInfo.Id = int.Parse(response[1]);
                    PlayerInfo.Name = response[2];
                    PlayerInfo.EMail = response[3];
                    Answer.text = "Login successful!";
                    //tutaj przejście do kolejnej sceny!
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
                Answer.text = "Error (Code: " + response[0] + ")! Please try again later!";
                break;
        }
    }

    private void toRegistration()
    {
        SceneManager.LoadScene(sceneName: "Reg");
    }
}
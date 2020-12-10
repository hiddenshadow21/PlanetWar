using System.Collections;
using System.Collections.Generic;
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

    private readonly string loginURL = "http://40.69.215.163/login.php";
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

        switch (www.downloadHandler.text)
        {
            case "S1":
                Answer.text = "Login successful!";
                //tutaj przejście do innej sceny!!
                break;
            case "E4":
            case "E3":
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
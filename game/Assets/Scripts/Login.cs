using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public InputField Email;
    public InputField Password;
    public Button Send;
    public Text Answer;
    private readonly string loginURL = "http://40.69.215.163/login.php";


    private void callLogin()
    {
        StartCoroutine(login());
    }

    private void OnEnable()
    {
        Send.onClick.AddListener(callLogin);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(Email.isFocused == true)
            {
                Password.Select();
            }
            else if(Password.isFocused == true)
            {
                Email.Select();
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
        WWW www = new WWW(loginURL, form);
        yield return www;

        switch (www.text)
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
                Answer.text = "Error (Code: " + www.text + ")! Please try again later!";
                break;
        }
    }
}
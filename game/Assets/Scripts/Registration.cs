using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Registration : MonoBehaviour
{
    public InputField Username;
    public InputField Email;
    public InputField Password;
    public InputField ConfirmPassword;
    public Button Send;
    public Text Answer;
    private readonly string registrationURL = "http://40.69.215.163/registration.php";


    private void callRegistration()
    {
        StartCoroutine(registration());
    }

    private void OnEnable()
    {
        Send.onClick.AddListener(callRegistration);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(Username.isFocused == true)
            {
                Email.Select();
            }
            else if(Email.isFocused == true)
            {
                Password.Select();
            }
            else if (Password.isFocused == true)
            {
                ConfirmPassword.Select();
            }
            else if (ConfirmPassword.isFocused == true)
            {
                Username.Select();
            }
        }

        Send.interactable = (Username.text != "" && Email.text != ""  && Password.text != "" && ConfirmPassword.text != "");

        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(Send.interactable == true)
            {
                callRegistration();
            } 
        }
    }

    private IEnumerator registration()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", Email.text);
        form.AddField("password", Password.text);
        form.AddField("confPassword", ConfirmPassword.text);
        form.AddField("username", Username.text);
        WWW www = new WWW(registrationURL, form);
        yield return www;

        switch (www.text)
        {
            case "S2":
                Answer.text = "New account created succesfully!";
                //tutaj przejście do innej sceny!!
                break;
            case "E6":
                Answer.text = "Username is too long! Maximum length is 20.";
                break;
            case "E7":
                Answer.text = "Email is incorrect! Please check the syntax.";
                break;
            case "E8":
                Answer.text = "Email address is too long! Maximum length is 20.";
                break;
            case "E9":
                Answer.text = "Password has the wrong length! Length is between 8 and 20.";
                break;
            case "E10":
                Answer.text = "Password has no number!";
                break;
            case "E11":
                Answer.text = "Both passwords are not equal!";
                break;
            case "E12":
                Answer.text = "Account with this email already exists!";
                break;
            case "E13":
                Answer.text = "Account with this username already exists!";
                break;
            default:
                Answer.text = "Error (Code: " + www.text + ")! Please try again later!";
                break;
        }
    }
}
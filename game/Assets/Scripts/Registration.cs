using System;
using System.Collections;
using System.Net.Mail;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Registration : MonoBehaviour
{ 
    public InputField Username; 
    public InputField Email; 
    public InputField Password; 
    public InputField ConfirmPassword; 
    public Button Send; 
    public Button ToLogin; 
    public Text Answer;

    private const string registrationURL = "http://40.69.215.163/logreg/registration.php";
    private const char fieldSeparator = ':';

    private void callRegistration()
    {
        StartCoroutine(registration());
    }

    private void Start()
    {
        Username.Select();
    }

    private void OnEnable()
    {
        Send.onClick.AddListener(callRegistration);
        ToLogin.onClick.AddListener(toLogin);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            switch (EventSystem.current.currentSelectedGameObject.name)
            {
                case "Username_input":
                    Email.Select();
                    break;
                case "Email_input":
                    Password.Select();
                    break;
                case "Password_input":
                    ConfirmPassword.Select();
                    break;
                case "PasswordConfirm_input":
                    if (Send.interactable == true)
                    {
                        Send.Select();
                    }
                    else
                    {
                        ToLogin.Select();
                    }   
                    break;
                case "Register_button":
                    ToLogin.Select();
                    break;
                case "ToLogin_button":
                    Username.Select();
                    break;
            }
        }

        Send.interactable = (Username.text != "" && Email.text != ""  && Password.text != "" && ConfirmPassword.text != "");

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(Send.interactable == true)
            {
                callRegistration();
            } 
        }
    }

    private string encryptData(string data)
    {
        byte[] salt = new byte[16];
        new RNGCryptoServiceProvider().GetBytes(salt);
        var rfc2989db = new Rfc2898DeriveBytes(data, salt, 10000);

        byte[] hash = rfc2989db.GetBytes(20);
        byte[] mergedData = new byte[36];

        Array.Copy(salt, 0, mergedData, 0, 16);
        Array.Copy(hash, 0, mergedData, 16, 20);

        string finalResult = Convert.ToBase64String(mergedData);
        return finalResult;
    }

    private string usernameTest()
    {
        if(Username.text.Length > 20)
        {
            return "R_UT_1";
        }

        if(Username.text.Contains(fieldSeparator.ToString()))
        {
            return "R_UT_2";
        }
        return "_SUCCESSFUL";
    }

    private string passwordTest()
    {
        if(Password.text.Length <= 7 || Password.text.Length > 20)
        {
            return "R_PT_1";
        }

        char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        if(Password.text.IndexOfAny(numbers) == -1)
        {
            return "R_PT_2";
        }

        if(Password.text.Contains(fieldSeparator.ToString()))
        {
            return "R_PT_3";
        }

        if (Password.text != ConfirmPassword.text)
        {
            return "R_PT_4";
        }
        return "_SUCCESSFUL";
    }

    private string emailTest()
    {
        if(Email.text.Contains(fieldSeparator.ToString()))
        {
            return "R_ET_1";
        }

        if(Email.text.Length > 40)
        {
            return "R_ET_2";
        }

        try
        {
            var address = new MailAddress(Email.text);

            if(address.Address == Email.text)
            {
                return "_SUCCESSFUL";
            }
            return "R_ET_3";
        }
        catch
        {
            return "R_ET_3";
        }
    }

    private IEnumerator registration()
    {
        bool inputInterpreterResult = inputInterpreter();
        if(inputInterpreterResult == true)
        {
            WWWForm form = new WWWForm();

            form.AddField("email", Email.text);
            form.AddField("password", encryptData(Password.text));
            form.AddField("username", Username.text);

            UnityWebRequest www = UnityWebRequest.Post(registrationURL, form);
            yield return www.SendWebRequest();

            outputInterpreter(www.downloadHandler.text);
        }   
    }

    private bool inputInterpreter()
    {
        string inputEmailResponse = emailTest();
        switch(inputEmailResponse)
        {
            case "R_ET_1":
                Answer.text = "Email contains characters that are not allowed.";
                return false;
            case "R_ET_2":
                Answer.text = "Email is too long.";
                return false;
            case "R_ET_3":
                Answer.text = "Email is incorrect! Please check the syntax.";
                return false;
        }

        string inputUsernameResponse = usernameTest();
        switch(inputUsernameResponse)
        {
            case "R_UT_1":
                Answer.text = "Username is too long! Maximum length is 20.";
                return false;
            case "R_UT_2":
                Answer.text = "Username contains characters that are not allowed.";
                return false;
        }

        string inputPasswordResponse = passwordTest();
        switch (inputPasswordResponse)
        {
            case "R_PT_1":
                Answer.text = "Password has the wrong length! Length is between 8 and 20.";
                return false;
            case "R_PT_2":
                Answer.text = "Password has no number!";
                return false;
            case "R_PT_3":
                Answer.text = "Password contains characters that are not allowed.";
                return false;
            case "R_PT_4":
                Answer.text = "Both passwords are not equal!";
                return false;
        }
        return true;
    }

    private void outputInterpreter(string serverResponse)
    {
        switch (serverResponse)
        {   
            case "R_CNA_SUCCESSFUL":
                Answer.text = "New account created succesfully! Please use link from your email to activate account!";
                //tutaj powrót do sceny logowania!!
                break;
            case "R_CNA_2":
                Answer.text = "Account with this username already exists!";
                break;
            case "R_CNA_3":
                Answer.text = "Account with this email already exists!";
                break;
            default:
                Answer.text = "Error (Code: " + serverResponse + ")! Please try again later!";
                break;
        }
    }

    private void toLogin()
    {
        SceneManager.LoadScene(sceneName: "Log");
    }
}
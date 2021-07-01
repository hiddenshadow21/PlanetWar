using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PasswordCheck : MonoBehaviour
{
    public InputField Password;
    public InputField ConfirmPassword;
    public Button ToLog;
    public Button PasswordVerify;
    public Text Answer;

    private const string passwordVerifyURL = "http://40.69.215.163/logreg/newPasswordVerify.php";
    private const char fieldSeparator = ':';

    private void callPasswordVerify()
    {
        PasswordVerify.interactable = false;
        StartCoroutine(passwordVerify());
    }

    private void Start()
    {
        Password.Select();
    }

    private void OnEnable()
    {
        PasswordVerify.onClick.AddListener(callPasswordVerify);
        ToLog.onClick.AddListener(toLog);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            switch (EventSystem.current.currentSelectedGameObject.name)
            {
                case "Password_input":
                    ConfirmPassword.Select();
                    break;
                case "ConfirmPassword_input":
                    if (PasswordVerify.interactable == true)
                    {
                        PasswordVerify.Select();
                    }
                    else
                    {
                        ToLog.Select();
                    }
                    break;
                case "PasswordVerify_button":
                    ToLog.Select();
                    break;
                case "ToLog_button":
                    Password.Select();
                    break;
            }
        }

        PasswordVerify.interactable = (Password.text != "" && ConfirmPassword.text != "");

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (PasswordVerify.interactable == true)
            {
                callPasswordVerify();
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

    private string passwordTest()
    {
        if (Password.text.Length <= 7 || Password.text.Length > 20)
        {
            return "PC_PT_1";
        }

        char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        if (Password.text.IndexOfAny(numbers) == -1)
        {
            return "PC_PT_2";
        }

        if (Password.text.Contains(fieldSeparator.ToString()))
        {
            return "PC_PT_3";
        }

        if (Password.text != ConfirmPassword.text)
        {
            return "PC_PT_4";
        }
        return "_SUCCESSFUL";
    }

    private IEnumerator passwordVerify()
    {
        bool inputInterpreterResult = inputInterpreter();
        if (inputInterpreterResult == true)
        {
            WWWForm form = new WWWForm();

            form.AddField("password", encryptData(Password.text));
            form.AddField("email", ValuesTransfer.Email);
            form.AddField("code", ValuesTransfer.Code);

            UnityWebRequest www = UnityWebRequest.Post(passwordVerifyURL, form);
            yield return www.SendWebRequest();

            outputInterpreter(www.downloadHandler.text);
        }
    }

    private bool inputInterpreter()
    {
        string inputPasswordResponse = passwordTest();
        switch (inputPasswordResponse)
        {
            case "PC_PT_1":
                Answer.text = "Password has the wrong length! Length is between 8 and 20.";
                return false;
            case "PC_PT_2":
                Answer.text = "Password has no number!";
                return false;
            case "PC_PT_3":
                Answer.text = "Password contains characters that are not allowed.";
                return false;
            case "PC_PT_4":
                Answer.text = "Both passwords are not equal!";
                return false;
        }
        return true;
    }

    private void outputInterpreter(string serverResponse)
    {
        switch (serverResponse)
        {
            case "NPV_SP_SUCCESSFUL":
                SceneManager.LoadScene("Log");
                break;
            default:
                if (serverResponse == "")
                {
                    Answer.text = "Error! Please check Your internet connection and try again later!";
                }
                else
                {
                    Answer.text = "Error (Code: " + serverResponse + ")! Please try again later!";
                }
                break;
        }
        PasswordVerify.interactable = true;
    }

    private void toLog()
    {
        SceneManager.LoadScene(sceneName: "Log");
    }
}

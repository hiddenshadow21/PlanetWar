using System.Collections;
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

    private IEnumerator registration()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", Email.text);
        form.AddField("password", Password.text);
        form.AddField("confPassword", ConfirmPassword.text);
        form.AddField("username", Username.text);

        UnityWebRequest www = UnityWebRequest.Post(registrationURL, form);
        yield return www.SendWebRequest();

        outputInterpreter(www.downloadHandler.text);
    }

    private void outputInterpreter(string serverResponse)
    {
        switch (serverResponse)
        {
            case "R_UT_1":
                Answer.text = "Username is too long! Maximum length is 20.";
                break;
            case "R_UT_2":
                Answer.text = "Username contains characters that are not allowed.";
                break;
            case "R_ET_1":
                Answer.text = "Email is incorrect! Please check the syntax.";
                break;
            case "R_ET_2":
                Answer.text = "Email address is too long! Maximum length is 20.";
                break;
            case "R_ET_3":
                Answer.text = "Email contains characters that are not allowed.";
                break;
            case "R_PT_1":
                Answer.text = "Password has the wrong length! Length is between 8 and 20.";
                break;
            case "R_PT_2":
                Answer.text = "Password has no number!";
                break;
            case "R_PT_3":
                Answer.text = "Password contains characters that are not allowed.";
                break;
            case "R_PT_4":
                Answer.text = "Both passwords are not equal!";
                break;
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
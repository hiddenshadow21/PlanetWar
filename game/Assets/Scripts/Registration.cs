using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Registration : MonoBehaviour
{
    public InputField Username; //0
    public InputField Email; //1
    public InputField Password; //2
    public InputField ConfirmPassword; //3
    public Button Send; //4
    public Button ToLogin; //5
    public Text Answer;

    private const string registrationURL = "http://40.69.215.163/registration.php";
    private int activeSwitchID;

    private void callRegistration()
    {
        StartCoroutine(registration());
    }

    private void Start()
    {
        Username.Select();
        activeSwitchID = 0;
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
            switch (activeSwitchID)
            {
                case 0:
                    Email.Select();
                    activeSwitchID = 1;
                    break;
                case 1:
                    Password.Select();
                    activeSwitchID = 2;
                    break;
                case 2:
                    activeSwitchID = 3;
                    ConfirmPassword.Select();
                    break;
                case 3:
                    if (Send.interactable == true)
                    {
                        activeSwitchID = 4;
                        Send.Select();
                    }
                    else
                    {
                        activeSwitchID = 5;
                        ToLogin.Select();
                    }   
                    break;
                case 4:
                    activeSwitchID = 5;
                    ToLogin.Select();
                    break;
                case 5:
                    activeSwitchID = 0;
                    Username.Select();
                    break;
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

        UnityWebRequest www = UnityWebRequest.Post(registrationURL, form);
        yield return www.SendWebRequest();

        switch (www.downloadHandler.text)
        {
            case "R_U_1":
                Answer.text = "Username is too long! Maximum length is 20.";
                break;
            case "R_U_3":
                Answer.text = "Username contains characters that are not allowed.";
                break;
            case "R_E_1":
                Answer.text = "Email is incorrect! Please check the syntax.";
                break;
            case "R_E_2":
                Answer.text = "Email address is too long! Maximum length is 20.";
                break;
            case "R_E_4":
                Answer.text = "Email contains characters that are not allowed.";
                break;
            case "R_P_1":
                Answer.text = "Password has the wrong length! Length is between 8 and 20.";
                break;
            case "R_P_2":
                Answer.text = "Password has no number!";
                break;
            case "R_P_3":
                Answer.text = "Password contains characters that are not allowed.";
                break;
            case "R_P_4":
                Answer.text = "Both passwords are not equal!";
                break;
            case "RS_1":
                Answer.text = "New account created succesfully!";
                //tutaj powrót do sceny logowania!!
                break;
            case "R_U_2":
                Answer.text = "Account with this username already exists!";
                break;
            case "R_E_3":
                Answer.text = "Account with this email already exists!";
                break;
            default:
                Answer.text = "Error (Code: " + www.downloadHandler.text + ")! Please try again later!";
                break;
        }
    }

    private void toLogin()
    {
        SceneManager.LoadScene(sceneName: "Log");
    }
}
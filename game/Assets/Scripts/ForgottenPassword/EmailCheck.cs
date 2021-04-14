using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EmailCheck : MonoBehaviour
{
    public InputField Email;
    public Button ToLog;
    public Button EmailVerify;
    public Text Answer;

    private const string emailVerifyURL = "http://40.69.215.163/logreg/newPasswordEmailVerify.php";

    private void callEmailVerify()
    {
        EmailVerify.interactable = false;
        StartCoroutine(emailVerify());
    }

    private void Start()
    {
        Email.Select();
    }

    private void OnEnable()
    {
        EmailVerify.onClick.AddListener(callEmailVerify);
        ToLog.onClick.AddListener(toLog);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            switch (EventSystem.current.currentSelectedGameObject.name)
            {
                case "Email_input":
                    if (EmailVerify.interactable == true)
                    {
                        EmailVerify.Select();
                    }
                    else
                    {
                        ToLog.Select();
                    }
                    break;
                case "EmailVerify_button":
                    ToLog.Select();
                    break;
                case "ToLog_button":
                    Email.Select();
                    break;
            }
        }

        EmailVerify.interactable = (Email.text != "");

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (EmailVerify.interactable == true)
            {
                callEmailVerify();
            }
        }
    }

    private IEnumerator emailVerify()
    {
        WWWForm form = new WWWForm();

        form.AddField("email", Email.text);

        UnityWebRequest www = UnityWebRequest.Post(emailVerifyURL, form);
        yield return www.SendWebRequest();

        outputInterpreter(www.downloadHandler.text);
    }

    private void outputInterpreter(string serverResponse)
    {
        switch (serverResponse)
        {
            case "NPEV_SVM_SUCCESSFUL":
                ValuesTransfer.Email = Email.text;
                SceneManager.LoadScene("CodeCheck");
                break;
            case "NPEV_SC_4":
                Answer.text = "This account is not verified!";
                break;
            case "NPEV_SC_2":
                Answer.text = "No restore option was found for this account!";
                break;

            default:
                Answer.text = "Error (Code: " + serverResponse + ")! Please try again later!";
                break;
        }
        EmailVerify.interactable = true;
    }

    private void toLog()
    {
        SceneManager.LoadScene(sceneName: "Log");
    }
}

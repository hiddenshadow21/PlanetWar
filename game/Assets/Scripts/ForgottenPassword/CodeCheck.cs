using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CodeCheck : MonoBehaviour
{
    public InputField Code;
    public Button ToLog;
    public Button CodeVerify;
    public Text Answer;

    private const string codeVerifyURL = "http://40.69.215.163/logreg/newPasswordCodeVerify.php";

    private void callCodeVerify()
    {
        CodeVerify.interactable = false;
        StartCoroutine(codeVerify());
    }

    private void Start()
    {
        Code.Select();
    }

    private void OnEnable()
    {
        CodeVerify.onClick.AddListener(callCodeVerify);
        ToLog.onClick.AddListener(toLog);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            switch (EventSystem.current.currentSelectedGameObject.name)
            {
                case "Code_input":
                    if (CodeVerify.interactable == true)
                    {
                        CodeVerify.Select();
                    }
                    else
                    {
                        ToLog.Select();
                    }
                    break;
                case "CodeVerify_button":
                    ToLog.Select();
                    break;
                case "ToLog_button":
                    Code.Select();
                    break;
            }
        }

        CodeVerify.interactable = (Code.text != "");

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (CodeVerify.interactable == true)
            {
                callCodeVerify();
            }
        }
    }

    private IEnumerator codeVerify()
    {
        WWWForm form = new WWWForm();

        form.AddField("code", Code.text);
        form.AddField("email", ValuesTransfer.Email);

        UnityWebRequest www = UnityWebRequest.Post(codeVerifyURL, form);
        yield return www.SendWebRequest();

        outputInterpreter(www.downloadHandler.text);
    }

    private void outputInterpreter(string serverResponse)
    {
        switch (serverResponse)
        {
            case "NPCV_VC_SUCCESSFUL":
                ValuesTransfer.Code = Code.text;
                SceneManager.LoadScene("PasswordCheck");
                break;
            case "NPCV_VC_2":
                Answer.text = "Invalid code!";
                break;
            default:
                Answer.text = "Error (Code: " + serverResponse + ")! Please try again later!";
                break;
        }
        CodeVerify.interactable = true;
    }

    private void toLog()
    {
        SceneManager.LoadScene(sceneName: "Log");
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LogRegPictures : MonoBehaviour
{
    public Image ContentImage;
    public Text ImageTitle;
    public Text ImageDescription;

    private const string dataUrl = "http://40.69.215.163/logreg/clientData/";
    private int maxPictureNumber = 0;
    private int pictureNumber = 0;
    private WebClient client = new WebClient();
    private List<Sprite> sprites = new List<Sprite>();
    private List<string> titles = new List<string>();
    private List<string> descriptions = new List<string>();

    void Start()
    {
        try
        {
            getImagesNumberFromServer();
            getDataFromServer();
            InvokeRepeating(nameof(setNewPicture), 1.0f, 10.0f);
        }
        catch(Exception e)
        {
            ImageTitle.text = "Connection error";
            ImageDescription.text = e.Message;
        }
    }

    private void getImagesNumberFromServer()
    {
        string output = client.DownloadString(new Uri(dataUrl + "getImagesNumber.php"));
        maxPictureNumber = int.Parse(output);
        if(maxPictureNumber == 0)
        {
            throw new Exception("Unable to get data from server");
        }
    }


    private void getDataFromServer()
    {
        for (int i = 0; i < maxPictureNumber; i++)
        {
            titles.Add(client.DownloadString(new Uri(dataUrl + i + "/title.txt")));
            descriptions.Add(client.DownloadString(new Uri(dataUrl + i + "/description.txt")));

            StartCoroutine(getImagesFromServer(i, (response) =>
            {
                sprites.Add(response);
            }));
        }
    }

    private IEnumerator getImagesFromServer(int index, System.Action<Sprite> callback)
    {
        using (var www = UnityWebRequestTexture.GetTexture(dataUrl + index + "/image.png"))
        {
            yield return www.SendWebRequest();

            if(www.isNetworkError || www.isHttpError)
            {
                throw new Exception("Unable to get data from server");
            }


            if (www.isDone)
            {
                var image = DownloadHandlerTexture.GetContent(www);
                var rect = new Rect(0, 0, 1920f, 1080f);
                Sprite sprite = Sprite.Create(image, rect, new Vector2(0,0));
                callback(sprite);
            }
        }
    }

    private void setNewPicture()
    {
        ContentImage.sprite = sprites[pictureNumber];
        ImageTitle.text = titles[pictureNumber];
        ImageDescription.text = descriptions[pictureNumber];

        if (pictureNumber < maxPictureNumber - 1)
        {
            pictureNumber++;
        }
        else
        {
            pictureNumber = 0;
        }
    }
}

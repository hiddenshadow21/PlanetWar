using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class ServerGameplay : NetworkBehaviour
{
    private int matchTime;
    NetworkRoomManagerExt roomManager;
    private System.Random rand = new System.Random();

    void Start()
    {
        roomManager = NetworkManager.singleton as NetworkRoomManagerExt;
        matchTime = roomManager.MatchTime;
        StartCoroutine(updateMatchTimeServer());

        initChatMessages();

        initSpawnPoints();

        initPoisonAreas();
    }

    [Server]
    private IEnumerator updateMatchTimeServer()
    {
        yield return new WaitForSeconds(1f);
        if (matchTime > 0)
        {
            matchTime--;
            UpdateMatchTimeClients();
            StartCoroutine(updateMatchTimeServer());
        }
        else
        {
            // TO DO
            foreach (var gP in roomManager.gamePlayers)
                gP.DisableComponents();
            // tutaj wyświetl tabelkę wyników
            // Po 5s przechodzi do pokoju
            StartCoroutine(endMatchWithDelay(5));
        }
    }

    [Server]
    private void UpdateMatchTimeClients()
    {
        foreach (var nc in roomManager.gamePlayers)
        {
            nc.TargetUpdateHudTimer(nc.connectionToClient, matchTime);
        }
    }

    [Server]
    private IEnumerator endMatchWithDelay(float t)
    {
        yield return new WaitForSeconds(t);
        roomManager.ServerChangeScene(roomManager.RoomScene);
    }

    #region Chat
    private const int chatNumber = 8;
    public List<string> Chats = new List<string>();
    private bool isChatUpdateRunning = true;

    [Server]
    private void initChatMessages()
    {
        for (int i = 0; i < chatNumber; i++)
        {
            Chats.Add("");
        }
        StartCoroutine(updateChatPositionPeriodically());
    }

    [Server]
    public void SendChatMessage(string username, string message)
    {
        if(username != "" && message != "")
        {
            isChatUpdateRunning = false;
            string chatFormat = username + "~" + message;
            updateChatPosition();
            Chats[0] = chatFormat;
            sendChatUpdateToClients();
        }
    }

    [Server]
    private IEnumerator updateChatPositionPeriodically()
    {
        while (true)
        {
            if (isChatUpdateRunning == false)
            {
                isChatUpdateRunning = true;
            }
            else
            {
                updateChatPosition();
                sendChatUpdateToClients();
            }
            yield return new WaitForSeconds(5);
        }   
    }

    [Server]
    private void updateChatPosition()
    {
        for (int i = chatNumber - 1; i > 0; i--)
        {
            Chats[i] = Chats[i - 1];
        }
        Chats[0] = "";
    }

    [Server]
    private void sendChatUpdateToClients()
    {
        foreach (var nc in roomManager.gamePlayers)
        {
            nc.TargetUpdateChat(nc.connectionToClient, Chats);
        }
    }
    #endregion

    #region Bonus
    public GameObject MedKit;
    public GameObject ArmorKit;
    public GameObject GunSpeedChanger;
    public GameObject CarpetBombing;
    public GameObject CarpetBombingShield;
    public GameObject PoisonAreaShield;
    private const int BonusCount = 6;
    private BonusSpawnPoint[] spawnPoints;

    [Server]
    private void initSpawnPoints()
    {
        var spawnPointsContainer = GameObject.FindGameObjectWithTag("BonusSpawnPoints");
        spawnPoints = spawnPointsContainer.GetComponentsInChildren<BonusSpawnPoint>();
        StartCoroutine(randomSpawnPoints());
    }

    [Server]
    private IEnumerator randomSpawnPoints(float time = 10)
    {
        yield return new WaitForSeconds(time);
       
        StartCoroutine(spawnBonus(rand.Next(5, 10)));
        StartCoroutine(randomSpawnPoints());
    }

    [Server]
    private IEnumerator spawnBonus(float refreshTime)
    {
        yield return new WaitForSeconds(refreshTime);

        int selectedId;
        do
        {
            selectedId = rand.Next(0, spawnPoints.Length);
        }
        while (spawnPoints[selectedId].IsSpawnPointUsed == true);

        int selectedBonusId = rand.Next(0, BonusCount);
        GameObject bonusGameObject = null;

        switch (selectedBonusId)
        {
            case 0: //MedKit
                bonusGameObject = Instantiate(MedKit, spawnPoints[selectedId].Coordinates, Quaternion.identity);
                break;
            case 1: //ArmorKit
                bonusGameObject = Instantiate(ArmorKit, spawnPoints[selectedId].Coordinates, Quaternion.identity);
                break;
            case 2: //GunSpeedChanger
                bonusGameObject = Instantiate(GunSpeedChanger, spawnPoints[selectedId].Coordinates, Quaternion.identity);
                break;
            case 3: //CarpetBombing
                bonusGameObject = Instantiate(CarpetBombing, spawnPoints[selectedId].Coordinates, Quaternion.identity);
                break;
            case 4: //CarpetBombingShield
                bonusGameObject = Instantiate(CarpetBombingShield, spawnPoints[selectedId].Coordinates, Quaternion.identity);
                break;
            case 5: //PoisonZoneShield
                bonusGameObject = Instantiate(PoisonAreaShield, spawnPoints[selectedId].Coordinates, Quaternion.identity);
                break;
        }

        int lifeTime = rand.Next(15, 20);
        if (bonusGameObject != null)
        {
            bonusGameObject.GetComponent<Bonus>().onPlayerTakeBonus += serverGameplay_onPlayerTakeBonus;
            bonusGameObject.GetComponent<Bonus>().Id = selectedId;
            NetworkServer.Spawn(bonusGameObject);
            spawnPoints[selectedId].IsSpawnPointUsed = true;

            StartCoroutine(destroyBonus(selectedId, lifeTime, bonusGameObject));
        }
    }


    private void serverGameplay_onPlayerTakeBonus(object sender, int e)
    {
        NetworkServer.Destroy((GameObject)sender);
        spawnPoints[e].IsSpawnPointUsed = false;
    }

    [Server]
    private IEnumerator destroyBonus(int selectedId, int lifeTime, GameObject bonusGameObject)
    {
        yield return new WaitForSeconds(lifeTime);
        NetworkServer.Destroy(bonusGameObject);
        spawnPoints[selectedId].IsSpawnPointUsed = false;
    }
    #endregion

    #region Posion Area
    public GameObject PoisonArea;
    private PoisonAreaSpawnPoint[] poisonAreas;
    private int poisonedAreaNumber = 0;
    private int allAreasNumber = 0;

    [Server]
    private void initPoisonAreas()
    {
        var poisonAreaContainer = GameObject.FindGameObjectWithTag("Planets");
        poisonAreas = poisonAreaContainer.GetComponentsInChildren<PoisonAreaSpawnPoint>();
        allAreasNumber = poisonAreas.Length;
        sendPoisonAreaUpdateToClients();

        StartCoroutine(spawnPoisonAreas());
    }

    [Server]
    private IEnumerator spawnPoisonAreas()
    {
        yield return new WaitForSeconds(rand.Next(10, 50));

        int selectedSpawn;
        do
        {
            selectedSpawn = rand.Next(0, poisonAreas.Length);
        }
        while (poisonAreas[selectedSpawn].IsSpawnPointUsed == true);

        Vector3 coordinates = new Vector3(poisonAreas[selectedSpawn].Coordinates.x, poisonAreas[selectedSpawn].Coordinates.y, 1);
        GameObject poisonArea = Instantiate(PoisonArea, coordinates, Quaternion.identity);

        poisonArea.GetComponent<PoisonArea>().onPoisonAreaDestroy += serverGameplay_onPoisonAreaDestroy;
        poisonArea.GetComponent<PoisonArea>().Id = selectedSpawn;
        poisonAreas[selectedSpawn].IsSpawnPointUsed = true;

        poisonedAreaNumber++;
        sendPoisonAreaUpdateToClients();

        NetworkServer.Spawn(poisonArea);

        StartCoroutine(spawnPoisonAreas());
    }

    private void serverGameplay_onPoisonAreaDestroy(object sender, int e)
    {
        NetworkServer.Destroy((GameObject)sender);
        poisonAreas[e].IsSpawnPointUsed = false;
        poisonedAreaNumber--;
        sendPoisonAreaUpdateToClients();
    }

    [Server]
    private void sendPoisonAreaUpdateToClients()
    {
        foreach (var nc in roomManager.gamePlayers)
        {
            nc.TargetUpdatePoisonArea(nc.connectionToClient, poisonedAreaNumber, allAreasNumber);
        }
    }
    #endregion
}

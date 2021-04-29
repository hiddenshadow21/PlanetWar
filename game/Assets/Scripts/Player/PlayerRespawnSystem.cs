using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRespawnSystem : NetworkBehaviour
{
    private Button spawnButton;
    private Canvas canvas;
    private bool isActive=false;

    private PlayerController playerController;

    private void Start()
    {
        canvas = Resources.FindObjectsOfTypeAll<Canvas>().Where(x => x.tag == "Respawn").FirstOrDefault();
        spawnButton = canvas.GetComponentInChildren<Button>();
        spawnButton.onClick.AddListener(delegate ()
        {
            SpawnPlayerLocal();
        });
        playerController = gameObject.GetComponent<PlayerController>();
    }

    private void SpawnPlayerLocal()
    {
        if (!isLocalPlayer)
            return;
        ToogleCanvas();
        var random = new Random();
        var spawnPoints = SpawnPoint.GetSpawnPoints();
        int k = (int)Random.Range(0, spawnPoints.Count);
        transform.position = spawnPoints[k];
        CmdSpawnPlayer(k);
        PlayerWeaponController playerWeaponController = gameObject.GetComponent<PlayerWeaponController>();
        playerWeaponController.CmdSpawnSelectedWeapons();
        playerWeaponController.CmdChangeActiveWeapon(0);
    }

    [Command]
    public void CmdSpawnPlayer(int k)
    {
        transform.position = SpawnPoint.GetSpawnPoints()[k];
        playerController.EnableComponents();
        RpcSpawnPlayer(k);
    }

    [ClientRpc]
    private void RpcSpawnPlayer(int k)
    {
        transform.position = SpawnPoint.GetSpawnPoints()[k];
        playerController.EnableComponents();
    }

    public void ToogleCanvas()
    {
        isActive = !isActive;
        canvas.gameObject.SetActive(isActive);
    }
}

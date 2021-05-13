using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BonusSpawnPoint : NetworkBehaviour
{
    public Vector3 Coordinates { get; private set; }
    public bool IsSpawnPointUsed = false;

    private void Start()
    {
        Coordinates = gameObject.transform.position;
    }
}

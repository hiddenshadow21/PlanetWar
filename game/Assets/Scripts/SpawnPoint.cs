using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private static List<Vector3> spawnPoints = new List<Vector3>(4);

    void Start()
    {
        spawnPoints.Add(transform.position);
    }

    public static List<Vector3> GetSpawnPoints()
    {
        return spawnPoints;
    }
}

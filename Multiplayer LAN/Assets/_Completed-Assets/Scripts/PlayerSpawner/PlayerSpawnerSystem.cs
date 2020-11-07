using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using System;

public class PlayerSpawnerSystem : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab = null;

    private static List<Transform> spawnPoints = new List<Transform>();

    private int nextIndex = 0;

    public static void AddSpawnPoint(Transform transform)
    {
        spawnPoints.Add(transform);

        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }

    public static void RemoveSpawnPoint(Transform transform)
    {
        spawnPoints.Remove(transform);
    }

    [Server]
    public Transform GetSpawnPoint()
    {
        if (nextIndex >= spawnPoints.Count) nextIndex = 0;
        Transform spawnPoint = spawnPoints[nextIndex];
        nextIndex++;
        return spawnPoint;
    }

    public Transform GetNextSpawnTransform()
    {
        if (nextIndex >= spawnPoints.Count) nextIndex = 0;
        Transform spawnPoint = spawnPoints[nextIndex];
        nextIndex++;
        return spawnPoint;
    }
}

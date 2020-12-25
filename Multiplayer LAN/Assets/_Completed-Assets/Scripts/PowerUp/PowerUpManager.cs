using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PowerUpManager : NetworkBehaviour
{
    public GameObject[] PowerUpPrefabs;
    public List<Transform> spawnPositions = new List<Transform>();

    public float MinTime = 6.0f, MaxTime = 10.0f;

    public static PowerUpManager Manager;

    void Awake()
    {
        Manager = FindObjectOfType<PowerUpManager>();
    }

    /**
    * Solo en el server, obtiene los puntos de spawn puestos para los
    * npcs enemigos y luego instancia el prefab de Enemy, ademas de 
    * indicar al server que esta creando ese objeto (NetworkServer.Spawn(prefab))
    */
    public override void OnStartServer()
    {
        Manager.Invoke(nameof(SpawnPowerUp), Random.Range(MinTime, MaxTime));
    }


    [Server]
    public void SpawnPowerUp()
    {
        foreach (Transform point in transform)
        {
            spawnPositions.Add(point);
        }

        int idx = Random.Range(0, spawnPositions.Count);
        Transform t = spawnPositions[idx];
        spawnPositions.RemoveAt(idx);
        //
        Vector3 spawnPosition = t.position;
        Quaternion spawnRotation =
            Quaternion.Euler(0.0f, Random.Range(0, 180), 0.0f);
        GameObject powerUp = Instantiate(PowerUpPrefabs[Random.Range(0, 3)], spawnPosition, spawnRotation);
        //
        NetworkServer.Spawn(powerUp);
        Manager.Invoke(nameof(SpawnPowerUp), Random.Range(MinTime, MaxTime));
    }


}

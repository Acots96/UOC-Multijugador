using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PowerUpManager : NetworkBehaviour
{
    public GameObject[] PowerUpPrefabs;
    public List<Transform> spawnPositions = new List<Transform>();

    /**
    * Solo en el server, obtiene los puntos de spawn puestos para los
    * npcs enemigos y luego instancia el prefab de Enemy, ademas de 
    * indicar al server que esta creando ese objeto (NetworkServer.Spawn(prefab))
    */
    public override void OnStartServer()
    {
        Invoke(nameof(SpawnPowerUp), Random.Range(2.5f, 8.0f));
    }


    [Server]
    void SpawnPowerUp()
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
    }

    [Server]
    private void OnDestroy()
    {
        Invoke(nameof(SpawnPowerUp), Random.Range(2.5f, 8.0f));
    }


}

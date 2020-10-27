using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class EnemySpawner : NetworkBehaviour {

    public GameObject enemyPrefab;
    public int numberOfEnemies;


    public override void OnStartServer() {
        List<Transform> spawnPositions = new List<Transform>();
        foreach (Transform t in transform)
            spawnPositions.Add(t);

        for (int i = 0; i < numberOfEnemies; i++) {
            int idx = Random.Range(0, spawnPositions.Count);
            Transform t = spawnPositions[idx];
            spawnPositions.RemoveAt(idx);
            //
            Vector3 spawnPosition = t.position;
            Quaternion spawnRotation = 
                Quaternion.Euler(0.0f, Random.Range(0, 180), 0.0f);
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, spawnRotation);
            //
            NetworkServer.Spawn(enemy);
        }
    }

}
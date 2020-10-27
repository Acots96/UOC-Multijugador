using UnityEngine;
using Mirror;

public class EnemySpawner : NetworkBehaviour {

    public GameObject enemyPrefab;
    public int numberOfEnemies;


    public override void OnStartServer() {
        Debug.Log(Application.persistentDataPath);
        if (Application.isEditor) {
            Debug.Log("SERVER? " + isServer);
        } else {
            System.IO.File.WriteAllText("D:/UOC/Multi/PEC2/logfile2.txt", "SERVER? " + isServer);
        }
        for (int i = 0; i < numberOfEnemies; i++) {
            Vector3 spawnPosition = 
                new Vector3(Random.Range(-8.0f, 8.0f), 0.0f, Random.Range(-8.0f, 8.0f));
            Quaternion spawnRotation = 
                Quaternion.Euler(0.0f, Random.Range(0, 180), 0.0f);
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, spawnRotation);
            //
            NetworkServer.Spawn(enemy);
        }
    }

}
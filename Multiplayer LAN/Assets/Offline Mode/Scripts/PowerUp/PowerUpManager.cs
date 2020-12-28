using System.Collections.Generic;
using UnityEngine;

namespace Offline
{
    public class PowerUpManager : MonoBehaviour
    {
        public GameObject[] PowerUpPrefabs;
        public List<Transform> spawnPositions = new List<Transform>();

        public float MinTime = 6.0f, MaxTime = 10.0f;

        public static PowerUpManager Manager;

        void Awake()
        {
            Manager = FindObjectOfType<PowerUpManager>();
        }

        void Start()
        {
            Manager.Invoke(nameof(SpawnPowerUp), Random.Range(MinTime, MaxTime));
        }


        public void SpawnPowerUp()
        {
            foreach (Transform point in transform)
            {
                spawnPositions.Add(point);
            }

            int idx = Random.Range(0, spawnPositions.Count);
            Transform t = spawnPositions[idx];
            spawnPositions.RemoveAt(idx);
            
            Vector3 spawnPosition = t.position;
            Quaternion spawnRotation =
                Quaternion.Euler(0.0f, Random.Range(0, 180), 0.0f);
            GameObject powerUp = Instantiate(PowerUpPrefabs[Random.Range(0, 3)], spawnPosition, spawnRotation);

            Manager.Invoke(nameof(SpawnPowerUp), Random.Range(MinTime, MaxTime));
        }


    }
}

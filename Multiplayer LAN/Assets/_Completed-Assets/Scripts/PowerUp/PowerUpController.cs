using System;
using UnityEngine;
using Mirror;

public class PowerUpController : NetworkBehaviour
{
    public float RotationSpeed = 15f;

    [Serializable]
    public enum PowerUpType
    {
        Bomb, Health, RapidShell
    }
    public PowerUpType PowerType;

    public float HealAmmount = 50f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
    }

    [Server]
    private void Start()
    {
        Destroy(gameObject, 12.0f); 
    }
}

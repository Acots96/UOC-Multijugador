using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;

public class LobbyMenu : NetworkManager
{
    private NetworkManager manager;
    public string serverIP = "localhost";

    [SerializeField] private GameObject playerSpawnerSystem = null;
    private PlayerSpawnerSystem playerSpawnerSystemInstance = null;

    void Awake()
    {
        manager = FindObjectOfType<NetworkManager>();
    }

    public void RunServer()
    {
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (!NetworkClient.active)
            {
                manager.StartServer();
            }
        }

        AddressData();
    }

    public void CreateGame()
    {
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (!NetworkClient.active)
            {
                manager.StartHost();
            }
        }

        AddressData();
    }

    public void JoinGame()
    {
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (!NetworkClient.active)
            {
                manager.networkAddress = serverIP;
                manager.StartClient();
            }
        }

        AddressData();
    }

    private void AddressData()
    {
        //if (NetworkServer.active)
        //{
        //    Debug.Log("Server: active. IP: " + manager.networkAddress + " - Transport: " + Transport.activeTransport);
        //}
        //else
        //{
        //    Debug.Log("Attempted to join server " + serverIP);
        //}

        //Debug.Log("Local IP Address: " + GetLocalIPAddress());

        //Debug.Log("//////////////");
    }

    private static string GetLocalIPAddress()
    {
        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName.Contains("_Complete-Game"))
        {
            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnerSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (playerSpawnerSystemInstance == null)
            playerSpawnerSystemInstance = GameObject.FindGameObjectWithTag("PlayerSpawner").GetComponent<PlayerSpawnerSystem>();
        
        Transform spawnPoint = playerSpawnerSystemInstance.GetSpawnPoint();
        GameObject playerPrefab = Instantiate(manager.playerPrefab, spawnPoint.position, spawnPoint.rotation);
        NetworkServer.AddPlayerForConnection(conn, playerPrefab);
    }
}

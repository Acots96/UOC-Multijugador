using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror.Discovery;

public class LobbyMenu : NetworkManager
{
    private NetworkManager manager;
    public string serverIP = "localhost";

    [SerializeField] private GameObject playerSpawnerSystem = null;
    private PlayerSpawnerSystem playerSpawnerSystemInstance = null;

    [SerializeField] private Image selectedColorImage;

    void Awake()
    {
        manager = FindObjectOfType<NetworkManager>();
        AwakeColorsButtons();
    }

    public void RunServer()
    {
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (!NetworkClient.active)
            {
                networkDiscovery.AdvertiseServer();
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
                networkDiscovery.AdvertiseServer();
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
        //Complete.GameManager.AddTank(playerPrefab.transform);
    }


    /**
     * Obtiene el color de cada boton y le pone el listener correspondiente
     * para que llame al metodo que cambia el color.
     * Tambien actualiza al primer color por defecto
     */
    public void AwakeColorsButtons() {
        GameObject[] btns = GameObject.FindGameObjectsWithTag("PlayerColorButton");
        for (int i = 0; i < btns.Length; i++) {
            GameObject b = btns[i];
            b.GetComponent<Button>().onClick.
                AddListener(delegate { ColorChanged(b.GetComponent<Image>().color); });
        }
        ColorChanged(Color.red);
    }

    /**
     * Guarda en PlayerPrefs el color para pasarlo al tanque en la escena de juego.
     */
    private void ColorChanged(Color c) {
        string s = c.r + ";" + c.g + ";" + c.b;
        PlayerPrefs.SetString("SelectedColor", s);
        selectedColorImage.color = c;
    }



    [SerializeField] private NetworkDiscovery networkDiscovery;

    public void FindServers() {
        networkDiscovery.StartDiscovery();
    }

    public void OnDiscoveredServer(ServerResponse info) {
        // Note that you can check the versioning to decide if you can connect to the server or not using this method
        Debug.Log("DISCOVERED SERVER: "+info.ToString());
        JoinGame();
    }

}

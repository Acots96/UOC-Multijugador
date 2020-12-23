using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror.Discovery;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class LobbyMenu : NetworkManager
{
    public static NetworkManager manager;
    //public string serverIP = "localhost";

    [SerializeField] private GameObject playerSpawnerSystem = null;
    private PlayerSpawnerSystem playerSpawnerSystemInstance = null;

    [SerializeField] private Image selectedColorImage;

    void Awake()
    {
        manager = FindObjectOfType<NetworkManager>();
        AwakeColorsButtons();
        gameType = GameType.WAN;
    }

    public void RunServer()
    {
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (!NetworkClient.active)
            {
                if (gameType == GameType.LAN)
                    networkDiscovery.AdvertiseServer();
                manager.StartServer();
            }
        }

        AddressData();
    }

    public void CreateGame()
    {
        if (gameType == GameType.Local) {
            SceneManager.LoadScene(LocalScene);
        } else {
            if (!NetworkClient.isConnected && !NetworkServer.active) {
                if (!NetworkClient.active) {
                    if (gameType == GameType.LAN)
                        networkDiscovery.AdvertiseServer();
                    manager.StartHost();
                }
            }

            AddressData();
        }
    }

    public void JoinGame()
    {
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (!NetworkClient.active)
            {
                if (gameType == GameType.LAN) {                    
                    manager.StartClient();
                } else {
                    //manager.networkAddress = serverIP;
                    if (!CheckValidIP()) {
                        InvalidIpText.text = "Invalid IP. Must be like XXX.XXX.XXX.XXX (0 <= XXX <= 255)";
                        return;
                    }
                    manager.networkAddress = IpField.text;
                    manager.StartClient();
                }
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

    [SerializeField] private TMP_InputField IpField;
    [SerializeField] private TextMeshProUGUI InvalidIpText;

    private bool CheckValidIP() {
        string ip = IpField.text;
        if (ip.Length < 7 || ip.Length > 15 || !ip.Contains("."))
            return false;

        string[] ipNums = ip.Split('.');
        if (ipNums.Length != 4)
            return false;

        int[] nums = new int[4];
        for (int i = 0; i < ipNums.Length; i++) {
            try { nums[i] = int.Parse(ipNums[i]); }
            catch (Exception e) { return false; }
            if (nums[i] < 0 || nums[i] > 255)
                return false;
        }
        return true;
    }



    private enum GameType { Local, LAN, WAN }

    [SerializeField] private List<Button> ToDisableOnLocalButtons;
    [SerializeField, Scene, FormerlySerializedAs("OfflineScene")] private string LocalScene;

    private GameType gameType;

    public void OnDropdownValueChanged(int value) {
        switch (value) {
            case 0: //Local
                foreach (Button b in ToDisableOnLocalButtons)
                    b.interactable = false;
                IpField.interactable = false;
                gameType = GameType.Local;
                break;
            case 1: //LAN
                foreach (Button b in ToDisableOnLocalButtons)
                    b.interactable = true;
                IpField.interactable = false;
                gameType = GameType.LAN;
                break;
            case 2: //WAN
                foreach (Button b in ToDisableOnLocalButtons)
                    b.interactable = true;
                IpField.interactable = true;
                gameType = GameType.WAN;
                break;
        }
    }


    public void FindServers() {
        if (gameType == GameType.LAN)
            networkDiscovery.StartDiscovery();
        else
            JoinGame();
    }
    public void OnServerFound(ServerResponse resp) {
        JoinGame();
    }

}

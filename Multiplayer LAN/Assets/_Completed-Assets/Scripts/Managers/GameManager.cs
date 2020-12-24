using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;
using System.Linq;

namespace Complete
{
    public class GameManager : NetworkBehaviour
    {
        public int m_NumRoundsToWin = 5;             // The number of rounds a single player has to win to win the game
        public float m_StartDelay = 3f;             // The delay between the start of RoundStarting and RoundPlaying phases
        public float m_EndDelay = 3f;               // The delay between the end of RoundPlaying and RoundEnding phases
        public CameraControl m_CameraControl;       // Reference to the CameraControl script for control during different phases
        public Text m_MessageText;                  // Reference to the overlay Text to display winning text, etc
        public GameObject m_TankPrefab;             // Reference to the prefab the players will control
        public TankManager[] m_Tanks;               // A collection of managers for enabling and disabling different aspects of the tanks

        public TankHealth[] allTanks;


        [SyncVar] public int m_RoundNumber;          // Variable sincronizada para saber el numero de rondas
        private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts
        private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends
        //private TankManager m_RoundWinner;          // Reference to the winner of the current round.  Used to make an announcement of who won
        //private TankManager m_GameWinner;           // Reference to the winner of the game.  Used to make an announcement of who won

        private TankController m_RoundWinner;          // Se cambia a Tank Controller para usar el código que implementamos
        private TankController m_GameWinner;           // Se cambia a Tank Controller para usar el código que implementamos


        private static GameManager Instance;
        public List<Transform> npcsTanks, playersTanks,TotalPlayersInGame;

        [SyncVar] public bool InRound = false;    // Variable sincronizada para saber si estamos en medio de una ronda

        public static bool IsTeamsGame { get; private set; }
        public GameObject TeamsMenu;
        public Text TeamsText;
        public enum GameTeam { NoTeam, Blue, Red }
        private int m_BlueWins, m_RedWins;


        private void Awake() {
            if (Instance) {
                Destroy(Instance);
                Instance = null;
            }
            Instance = this;

            npcsTanks = new List<Transform>();
            playersTanks = new List<Transform>();

            if (PlayerPrefs.GetInt("IsTeamsGame") == 1) {
                IsTeamsGame = true;
                TeamsMenu.SetActive(true);
            }
        }

        private void Start()
        {
            // Create the delays so they only have to be made once
            m_StartWait = new WaitForSeconds(m_StartDelay);
            m_EndWait = new WaitForSeconds(m_EndDelay);

            SpawnAllTanks();
            SetCameraTargets();

            StartCoroutine(WaitingPlayers());
        }


        private void SpawnAllTanks()
        {
            // For all the tanks...
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                // ... create them, set their player number and references needed for control
                m_Tanks[i].m_Instance =
                    Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
                m_Tanks[i].m_PlayerNumber = i + 1;
                m_Tanks[i].Setup();
            }
        }


        /**
         * Metodos llamados desde TankController cuando entra o sale
         * una nueva instancia de un tanque, para ponerlo en las dos listas
         * que tiene en cuenta el CameraController para enfocar a todos 
         * los tanques.
         * Siempre se comprueba que el tanque ya exista (o no) en las
         * listas antes de realizar operaciones, para evitar duplicados.
         */

        //    Método para comprobar si el round esta activo
        public static bool GetOnRound()
        {
            return Instance.InRound;
        }

        public static void TogglePlayerTank(Transform player, bool status)
        {
            Instance.RpcToggleTank(player, status);
            if (!Instance.isLocalPlayer)
            {                
                Instance.ToggleTank(player, status);
            }
        }

        private static void AddPlayer(Transform player) {
            if (!Instance.playersTanks.Contains(player))
            {
                Instance.playersTanks.Add(player);
                if (!Instance.TotalPlayersInGame.Contains(player))
                    Instance.TotalPlayersInGame.Add(player);
            }
            if (IsTeamsGame)
                UpdateTeamsText();
        }
        private static void RemovePlayer(Transform player) {
            if (Instance.playersTanks.Contains(player))
                Instance.playersTanks.Remove(player);
            if (IsTeamsGame)
                UpdateTeamsText();
        }
        private static void AddEnemy(Transform enemy) {
            if (!Instance.npcsTanks.Contains(enemy))
                Instance.npcsTanks.Add(enemy);
        }

        private static void RemoveEnemy(Transform enemy) {
            if (Instance.npcsTanks.Contains(enemy))
                Instance.npcsTanks.Remove(enemy);
        }
        public static void AddTank(Transform tank) {
            if (tank.tag.Equals("Enemy"))
                AddEnemy(tank);
            else if (tank.tag.Equals("Player"))
                AddPlayer(tank);
            SetCameraTargets();
        }

        [Server]
        public void ToggleTank(Transform tank, bool status)
        {
            if (status)
                tank.gameObject.SetActive(status);
            else
                tank.gameObject.SetActive(status);
        }

        [ClientRpc]
        public void RpcToggleTank(Transform tank, bool status)
        {
            if(status)
            tank.gameObject.SetActive(status);
            else
            tank.gameObject.SetActive(status);
        }

        public static void RemoveTank(Transform tank) {
            if (tank.tag.Equals("Enemy"))
                RemoveEnemy(tank);
            else if (tank.tag.Equals("Player"))
                RemovePlayer(tank);
            SetCameraTargets();
        }
        private static void SetEnemies(List<Transform> npcs) {
            Instance.npcsTanks = npcs;
            SetCameraTargets();
        }

        public static List<Transform> GetNpcsTanks() {
            return Instance.npcsTanks;
        }

        public static List<Transform> GetPlayersTanks() {
            return Instance.playersTanks;
        }

        //Método donde se organiza la cámara
        public static void SetCameraTargets()
        {
            //Se obtiene la lista de objetos que tienen salud, es decir los tanques incluyendo los NPCS
            Instance.allTanks = FindObjectsOfType<TankHealth>();

            //Se asigna a la variable la cantidad de los objetos que estan activos en el momento
            Transform[] targets = new Transform[Instance.allTanks.Length];

            // For each of these transforms...
            for (int i = 0; i < Instance.allTanks.Length; i++)
            {
                // ... set it to the appropriate tank transform
                targets[i] = Instance.allTanks[i].transform;
            }

            // These are the targets the camera should follow
            Instance.m_CameraControl.m_Targets = targets;
            //Debug.Log("Tank Number" + tanks.Length);
        }


        // This is called from start and will run each phase of the game one after another
        private IEnumerator GameLoop()
        {

            //yield return StartCoroutine(WaitingPlayers());

            // Start off by running the 'RoundStarting' coroutine but don't return until it's finished
            yield return StartCoroutine(RoundStarting());

            // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished
            yield return StartCoroutine (RoundPlaying());

            // Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished
            yield return StartCoroutine(RoundEnding());

            // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found
            if (m_GameWinner != null)
            {
                // If there is a game winner, restart the level
                //SceneManager.LoadScene (0);
                if (isServer)
                    StopServer();
            }
            else
            {
                // If there isn't a winner yet, restart this coroutine so the loop continues
                // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end
                StartCoroutine (GameLoop());
            }
        }


        void StopServer()
        {
            //SceneManager.LoadScene(1);
            LobbyMenu.manager.StopServer();
            LobbyMenu.manager.StopHost();
        }


        private IEnumerator RoundStarting()
        {
            // As soon as the round starts reset the tanks and make sure they can't move
            if (isServer)
                ResetAllTanks();

            DisableTankControl();

            // Snap the camera's zoom and position to something appropriate for the reset tanks
            m_CameraControl.SetStartPositionAndSize();

            // Increment the round number and display text showing the players what round it is
            

            m_MessageText.text = "ROUND " + m_RoundNumber;

            // Wait for the specified length of time until yielding control back to the game loop
            yield return m_StartWait;
            //yield return null;
        }


        private IEnumerator WaitingPlayers()
        {
            // As soon as the round begins playing let the players control the tanks
            DisableTankControl();

            // Clear the text from the screen
            m_MessageText.text = "Waiting for More Players";

            // While there is not one tank left...
            while (!MoreThanOneTank())
            {
                // ... return on the next frame
                yield return null;
            }

            StartCoroutine(GameLoop());
        }

        private IEnumerator RoundPlaying()
        {
            // As soon as the round begins playing let the players control the tanks
            EnableTankControl();

            // Clear the text from the screen
            m_MessageText.text = string.Empty;

            // While there is not one tank left...
            while (!OneTankLeft())
            {
                // ... return on the next frame
                yield return null;
            }
        }


        [Server]
        void IncreaseRoundNumber()
        {
            m_RoundNumber++;
        }


        [Server]
        void IncreaseWinsNumber(TankController tank)
        {
            tank.m_Wins++;
        }

        [ClientRpc]
        void RpcIncreaseRoundNumber()
        {
            m_RoundNumber++;
        }

        [ClientRpc]
        void RpcIncreaseWinsNumber(TankController tank)
        {
            tank.m_Wins++;
        }

        private IEnumerator RoundEnding()
        {
            // Stop tanks from moving
            DisableTankControl();

            // Clear the winner from the previous round
            m_RoundWinner = null;

            // See if there is a winner now the round is over
            m_RoundWinner = GetRoundWinner();

            if (isServer)
                if (isLocalPlayer)
                    RpcIncreaseRoundNumber();
                else
                    IncreaseRoundNumber();

            // If there is a winner, increment their score
            if (m_RoundWinner != null)
            {
                if (isServer)
                {
                    if (isLocalPlayer)
                    {
                        RpcIncreaseWinsNumber(m_RoundWinner);
                    }
                    else
                    {
                        IncreaseWinsNumber(m_RoundWinner);
                    }
                }
            }

            //Para evitar que no haya sincronización en el texto y la variable
            yield return new WaitForSeconds(0.2f);


            // Now the winner's score has been incremented, see if someone has one the game
            m_GameWinner = GetGameWinner();

            // Get a message based on the scores and whether or not there is a game winner and display it
            string message = EndMessage();
            m_MessageText.text = message;

            // Wait for the specified length of time until yielding control back to the game loop
            yield return m_EndWait;
        }

        //Nuevo metodo que solo empieza la partida cuando hay un cliente
        private bool MoreThanOneTank()
        {
            // Skip all rounds logic

            // Start the count of tanks left at zero.
            if (!IsTeamsGame) {
                int numTanksLeft = 0;
                foreach (Transform playerTank in playersTanks) {
                    numTanksLeft++;
                }
                // If there are one or fewer tanks remaining return true, otherwise return false.
                return numTanksLeft >= 2;
            } else {
                int blues = 0, reds = 0;
                playersTanks.RemoveAll(tank => tank == null);
                foreach (Transform tr in playersTanks) {
                    if (tr == null)
                        continue;
                    if (tr.GetComponent<TankController>().IsBlueTeam)
                        blues++;
                    else
                        reds++;
                }
                UpdateTeamsText();
                if (blues == 2 && reds == 2) {
                    GameObject[] btns = GameObject.FindGameObjectsWithTag("PlayerColorButton");
                    foreach (GameObject btn in btns)
                        btn.GetComponent<Button>().interactable = false;
                    TeamsMenu.SetActive(false);
                    return true;
                }
                return false;
            }
        }


        // This is used to check if there is one or fewer tanks remaining and thus the round should end
        private bool OneTankLeft()
        {
            // Skip all rounds logic

            if (!IsTeamsGame) {
                // Start the count of tanks left at zero.
                int numTanksLeft = 0;
                // Go through all the tanks...
                for (int i = 0; i < m_Tanks.Length; i++) {
                    // ... and if they are active, increment the counter.
                    if (m_Tanks[i].m_Instance.activeSelf)
                        numTanksLeft++;
                }
                // If there are one or fewer tanks remaining return true, otherwise return false.
                return numTanksLeft <= 1;
            } else {
                bool blue = false, red = false;
                // Go through all the tanks to see if there only remains one team
                for (int i = 0; i < 4; i++) {
                    if (playersTanks[i].gameObject.activeSelf) {
                        TankController t = playersTanks[i].GetComponent<TankController>();
                        blue |= t.IsBlueTeam;
                        red |= !t.IsBlueTeam;
                    }
                }
                if (blue ^ red) {
                    if (blue)
                        m_BlueWins++;
                    else
                        m_RedWins++;
                    return true;
                }
                return false;
            }
        }

        //Ajuste para almacenar el ganador de la ronda
        private TankController GetRoundWinner()
        {

            foreach (Transform playerTank in playersTanks)
            {
                if (playerTank.gameObject.activeSelf)
                {
                    return playerTank.GetComponent<TankController>();
                }
            }

            // If none of the tanks are active it is a draw so return null
            return null;
        }

        //Ajuste para almacenar el ganador
        private TankController GetGameWinner()
        {
            // Go through all the tanks...
            foreach (Transform playerTank in playersTanks)
            {
                // ... and if one of them has enough rounds to win the game, return it
                if (playerTank.GetComponent<TankController>().m_Wins == m_NumRoundsToWin)
                {
                    return playerTank.GetComponent<TankController>();
                }
            }

            // If no tanks have enough rounds to win, return null
            return null;
        }

        //Ajuste para Mensaje Final
        private string EndMessage()
        {
            // By default when a round ends there are no winners so the default end message is a draw
            string message = "DRAW!";

            // If there is a winner then change the message to reflect that
            if (m_RoundWinner != null) {
                if (!IsTeamsGame) {
                    message = m_RoundWinner.GetComponent<TankNaming>().currentName + " WINS THE ROUND!";
                } else {
                    string colored = "";
                    if (m_RoundWinner.IsBlueTeam)
                        colored = "<color=#0000FF>BLUE</color>";
                    else
                        colored = "<color=#FF0000>RED</color>";
                    message = "TEAM " + colored + " WINS THE ROUND!";
                }
            }

            // Add some line breaks after the initial message
            message += "\n\n\n\n";

            // Go through all the tanks and add each of their scores to the message
            if (!IsTeamsGame) {
                foreach (Transform playerTank in TotalPlayersInGame) {
                    message += playerTank.GetComponent<TankNaming>().currentName + ": " + playerTank.GetComponent<TankController>().m_Wins + " WINS\n";
                }
            } else {
                message += "<color=#" + ColorUtility.ToHtmlStringRGB(Color.blue) + ">BLUE</color>: " + m_BlueWins + " WINS\n";
                message += "<color=#" + ColorUtility.ToHtmlStringRGB(Color.red) + ">RED</color>: " + m_RedWins + " WINS\n";
            }

            // If there is a game winner, change the entire message to reflect that
            if (m_GameWinner != null) {
                if (!IsTeamsGame) {
                    message = m_GameWinner.GetComponent<TankNaming>().currentName + " WINS THE GAME!";
                    NetworkIdentity playerIdentity = m_GameWinner.GetComponent<NetworkIdentity>();
                    UpdatePlayerStats(playerIdentity.connectionToClient);
                } else {
                    TankManager tm = m_Tanks.First((tank) => tank.m_Instance.GetComponent<TankController>().Equals(m_RoundWinner));
                    message = "TEAM " + tm.m_ColoredPlayerText + " WINS THE GAME!";
                }
            }

            return message;
        }

        [TargetRpc]
        void UpdatePlayerStats(NetworkConnection target)
        {
            PlayfabController._instance.SetStats();
        }


        //Reactiva los objetos de los tanques jugadores que se han unido a la partida
        private void ResetAllTanks()
        {

            foreach (Transform playerTank in TotalPlayersInGame)
            {
                playerTank.GetComponent<TankHealth>().RpcRandomPos();
                if (!playerTank.gameObject.activeSelf)
                {
                    RpcToggleTank(playerTank, true);

                }
            }
        }

        //Activa el control del tanque a partir de la variable de estado InRound
        private void EnableTankControl()
        {
            Instance.InRound = true;
        }

        
        //Desactiva el control del tanque a partir de la variable de estado InRound
        private void DisableTankControl()
        {
            Instance.InRound = false;
        }




        public static void UpdateTeamsText() {
            int blues = 0, reds = 0;
            foreach (Transform tr in Instance.playersTanks) {
                TankController c = tr.GetComponent<TankController>();
                if (c.IsBlueTeam)
                    blues++;
                else
                    reds++;
            }
            Instance.TeamsText.text = "<color=#0000FF>Team blue: " + blues + "/2</color>";
            Instance.TeamsText.text += "\n";
            Instance.TeamsText.text += "<color=#FF0000>Team red: " + reds + "/2</color>";
        }

    }
}
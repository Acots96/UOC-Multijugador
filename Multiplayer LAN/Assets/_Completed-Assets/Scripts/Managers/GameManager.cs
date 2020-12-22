using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

namespace Complete
{
    public class GameManager : NetworkBehaviour
    {
        [SyncVar] public int m_NumRoundsToWin = 5;   // The number of rounds a single player has to win to win the game
        public float m_StartDelay = 3f;             // The delay between the start of RoundStarting and RoundPlaying phases
        public float m_EndDelay = 3f;               // The delay between the end of RoundPlaying and RoundEnding phases
        public CameraControl m_CameraControl;       // Reference to the CameraControl script for control during different phases
        public Text m_MessageText;                  // Reference to the overlay Text to display winning text, etc
        public GameObject m_TankPrefab;             // Reference to the prefab the players will control
        public TankManager[] m_Tanks;               // A collection of managers for enabling and disabling different aspects of the tanks

        public TankHealth[] allTanks;


        [SyncVar] public int m_RoundNumber;                  // Which round the game is currently on
        private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts
        private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends
        //private TankManager m_RoundWinner;          // Reference to the winner of the current round.  Used to make an announcement of who won
        //private TankManager m_GameWinner;           // Reference to the winner of the game.  Used to make an announcement of who won

        private TankController m_RoundWinner;          // Reference to the winner of the current round.  Used to make an announcement of who won
        private TankController m_GameWinner;           // Reference to the winner of the game.  Used to make an announcement of who won


        private static GameManager Instance;
        public List<Transform> npcsTanks, playersTanks,TotalPlayersInGame;


        [SyncVar] public bool InRound = false;
        /*

                //Método en el que se añade el tanque a la SyncList
                public static void AddTankToMatch(Transform tank)
                {
                    if (tank != null)
                    {
                        Instance.Players.Add(tank);
                        Instance.PlayersTest.Add(tank);
                    }
                }*/

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

        private void Awake() {
            if (Instance) {
                Destroy(Instance);
                Instance = null;
            }
            Instance = this;

            //npcsTanks = new List<Transform>();
            //playersTanks = new List<Transform>();
        }

        private void Start()
        {
            // Create the delays so they only have to be made once
            m_StartWait = new WaitForSeconds(m_StartDelay);
            m_EndWait = new WaitForSeconds(m_EndDelay);

            SpawnAllTanks();
            SetCameraTargets();

            // Once the tanks have been created and the camera is using them as targets, start the game
            //StartCoroutine (GameLoop ());
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

        public static bool GetOnRound()
        {
            return Instance.InRound;
        }

        [Server]
        public static void TogglePlayerTank(Transform player, bool status)
        {
            Instance.RpcToggleTank(player, status);
        }

        private static void AddPlayer(Transform player) {
            if (!Instance.playersTanks.Contains(player))
            {
                Instance.playersTanks.Add(player);
                if (!Instance.TotalPlayersInGame.Contains(player))
                    Instance.TotalPlayersInGame.Add(player);
            }
        }
        private static void RemovePlayer(Transform player) {
            if (Instance.playersTanks.Contains(player))
                Instance.playersTanks.Remove(player);
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

        //Método anterior de Setear Cámara
/*        private void SetCameraTargets()
        {
            // Create a collection of transforms the same size as the number of tanks
            //Transform[] targets = new Transform[m_Tanks.Length + npcsTanks.Count];
            List<Transform> targets = new List<Transform>();

            // For each of these transforms...
            *//*for (int i = 0; i < m_Tanks.Length; i++)
            {
                // ... set it to the appropriate tank transform
                targets.Add(m_Tanks[i].m_Instance.transform);
            }*//*

            for (int i = 0; i < playersTanks.Count; i++) {
                targets.Add(playersTanks[i]);
            }

            for (int i = 0; i < npcsTanks.Count; i++) {
                //Debug.Log(npcsTanks[i]);
                targets.Add(npcsTanks[i]);
            }

            //foreach (Transform tr in targets)
            //    Debug.Log(tr.gameObject);

            // These are the targets the camera should follow
            m_CameraControl.m_Targets = targets.ToArray();
        }*/

        [ClientRpc]
        public void RpcLaunchRound()
        {
            StartCoroutine(GameLoop());
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
            GameObject.Find("NetworkManager").GetComponent<LobbyMenu>().manager.StopServer();
            GameObject.Find("NetworkManager").GetComponent<LobbyMenu>().manager.StopHost();
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

            /*            if(isServer)
                        RpcLaunchRound();*/

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


        private IEnumerator RoundEnding()
        {
            // Stop tanks from moving
            DisableTankControl();

            if (isServer)
                m_RoundNumber++;

            // Clear the winner from the previous round
            m_RoundWinner = null;

            // See if there is a winner now the round is over
            m_RoundWinner = GetRoundWinner();

            // If there is a winner, increment their score
            if (m_RoundWinner != null)
            {
                if (isServer)
                    m_RoundWinner.m_Wins++;
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


        private bool MoreThanOneTank()
        {
            // Skip all rounds logic

            // Start the count of tanks left at zero.
            int numTanksLeft = 0;

            foreach (Transform playerTank in playersTanks)
            {
                numTanksLeft++;
            }

            // If there are one or fewer tanks remaining return true, otherwise return false.
            return numTanksLeft >= 2;
        }


        // This is used to check if there is one or fewer tanks remaining and thus the round should end
        private bool OneTankLeft()
        {
            // Skip all rounds logic

           // Start the count of tanks left at zero.
            int numTanksLeft = 0;

            foreach (Transform playerTank in playersTanks)
            {
                numTanksLeft++;
            }

            // Go through all the tanks...
/*            for (int i = 0; i < m_Tanks.Length; i++)
            {
                // ... and if they are active, increment the counter.
                if (m_Tanks[i].m_Instance.activeSelf)
                    numTanksLeft++;
            }*/

            // If there are one or fewer tanks remaining return true, otherwise return false.
            return numTanksLeft <= 1; 

            //return false;
        }


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

        // This function is to find out if there is a winner of the round
        // This function is called with the assumption that 1 or fewer tanks are currently active
        /*        private TankManager GetRoundWinner()
                {

                    // Go through all the tanks...
                    for (int i = 0; i < m_Tanks.Length; i++)
                    {
                        // ... and if one of them is active, it is the winner so return it
                        if (m_Tanks[i].m_Instance.activeSelf)
                        {
                            return m_Tanks[i];
                        }
                    }

                    // If none of the tanks are active it is a draw so return null
                    return null;
                }*/


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


        // This function is to find out if there is a winner of the game
        /*        private TankManager GetGameWinner()
                {
                    // Go through all the tanks...
                    for (int i = 0; i < m_Tanks.Length; i++)
                    {
                        // ... and if one of them has enough rounds to win the game, return it
                        if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                        {
                            return m_Tanks[i];
                        }
                    }

                    // If no tanks have enough rounds to win, return null
                    return null;
                }*/


        private string EndMessage()
        {
            // By default when a round ends there are no winners so the default end message is a draw
            string message = "DRAW!";

            // If there is a winner then change the message to reflect that
            if (m_RoundWinner != null)
                message = m_RoundWinner.GetComponent<TankNaming>().currentName + " WINS THE ROUND!";

            // Add some line breaks after the initial message
            message += "\n\n\n\n";

            // Go through all the tanks and add each of their scores to the message
            foreach (Transform playerTank in TotalPlayersInGame)
            {
                message += playerTank.GetComponent<TankNaming>().currentName + ": " + playerTank.GetComponent<TankController>().m_Wins + " WINS\n";
            }

            // If there is a game winner, change the entire message to reflect that
            if (m_GameWinner != null)
                message = m_GameWinner.GetComponent<TankNaming>().currentName + " WINS THE GAME!";

            return message;
        }

        // Returns a string message to display at the end of each round
/*        private string EndMessage()
        {
            // By default when a round ends there are no winners so the default end message is a draw
            string message = "DRAW!";

            // If there is a winner then change the message to reflect that
            if (m_RoundWinner != null)
                message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

            // Add some line breaks after the initial message
            message += "\n\n\n\n";

            // Go through all the tanks and add each of their scores to the message
            for (int i = 0; i < m_Tanks.Length; i++)
            {
                message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
            }

            // If there is a game winner, change the entire message to reflect that
            if (m_GameWinner != null)
                message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

            return message;
        }*/


        // This function is used to turn all the tanks back on and reset their positions and properties
        private void ResetAllTanks()
        {

            foreach (Transform playerTank in TotalPlayersInGame)
            {
                if(!playerTank.gameObject.activeSelf)
                TogglePlayerTank(playerTank, true);
            }

/*            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].Reset();
            }*/
        }


        private void EnableTankControl()
        {
            Instance.InRound = true;

/*            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].EnableControl();
            }*/
        }


        private void DisableTankControl()
        {
            Instance.InRound = false;

/*            for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].DisableControl();
            }*/
        }
    }
}
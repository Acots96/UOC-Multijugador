using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

namespace Offline
{
    public class GameManager : MonoBehaviour
    {
        public int m_NumRoundsToWin = 5;            // The number of rounds a single player has to win to win the game.
        public float m_StartDelay = 3f;             // The delay between the start of RoundStarting and RoundPlaying phases.
        public float m_EndDelay = 3f;               // The delay between the end of RoundPlaying and RoundEnding phases.
        public CameraControl m_CameraControl;       // Reference to the CameraControl script for control during different phases.
        public Text m_MessageText;                  // Reference to the overlay Text to display winning text, etc.
        public GameObject m_TankPrefab;             // Reference to the prefab the players will control.
        public TankManager[] m_Tanks;               // A collection of managers for enabling and disabling different aspects of the tanks.

        public Transform[] m_AvailableSpawnPoints;
        
        public GameObject m_MultiplayerEventSystemPrefab;
        public UiButtonManager m_UiButtonManager;
        public GameObject m_MaxPlayerNumReachedMessageGO;

        private int m_RoundNumber;                  // Which round the game is currently on.
        private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts.
        private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends.
        private TankManager m_RoundWinner;          // Reference to the winner of the current round.  Used to make an announcement of who won.
        private TankManager m_GameWinner;           // Reference to the winner of the game.  Used to make an announcement of who won.

        private Camera m_MainCamera;

        private int m_ActualPlayersNum;
        private bool m_MaxPlayerNumReachedMessageShowing = false;

        public bool[] m_SpawnPointsInUse;

        //
        public GameObject m_StartGamePopup, m_StartGamePopupTeams;
        private bool isTeamsGame;
        public enum GameTeam { NoTeam, Blue, Red }
        private int m_BlueWins, m_RedWins;

        private void Start()
        {
            // Create the delays so they only have to be made once.
            m_StartWait = new WaitForSeconds (m_StartDelay);
            m_EndWait = new WaitForSeconds (m_EndDelay);

            // Subscribing events to script methods
            m_UiButtonManager.OnStartGame += StartGameMethod;
            m_CameraControl.OnTurnOffSplitScreen += TurnOffTankCameras;
            m_CameraControl.OnTurnOnSplitScreen += TurnOnTankCamerasAndRefreshSplitScreen;

            m_SpawnPointsInUse = Enumerable.Repeat(false, m_AvailableSpawnPoints.Length).ToArray();

            //
            if (PlayerPrefs.GetInt("IsTeamsGame") == 1) {
                isTeamsGame = true;
                m_StartGamePopup.SetActive(false);
                m_StartGamePopupTeams.SetActive(true);
                m_UiButtonManager.mainMenuGO = m_StartGamePopupTeams;
            }
        }

        // When CameraControl invokes the method, it enables tank cameras and makes a refresh of
        // the split screen divisions
        private void TurnOnTankCamerasAndRefreshSplitScreen()
        {
            for (int i = 0; i < m_ActualPlayersNum; i++)
            {
                m_Tanks[i].m_Instance.GetComponentInChildren<Camera>().enabled = true;
            }
            RefreshSplitScreenCameras();
        }

        // When CameraControl invokes the method, it disables tank cameras
        private void TurnOffTankCameras()
        {
            for (int i = 0; i < m_ActualPlayersNum; i++)
            {
                m_Tanks[i].m_Instance.GetComponentInChildren<Camera>().enabled = false;
            }
        }

        // UiButtonManager triggers this event with the number of players as parameter
        private void StartGameMethod(int playerNum, List<bool> playersAreBlue)
        {
            m_ActualPlayersNum = playerNum;

            SpawnAllTanks(playersAreBlue);
            SetCameraTargets();

            m_CameraControl.SetActualTargetNumber(playerNum);

            // Once the tanks have been created and the camera is using them as targets, start the game.
            StartCoroutine(GameLoop());
        }

        // Adds camera to the specified by index tank
        private void AddCamera(int idx, Camera mainCamera)
        {
            GameObject childCam = new GameObject("Camera" + (idx + 1));
            Camera newCam = childCam.AddComponent<Camera>();
            newCam.CopyFrom(mainCamera);

            // hardcoded clipplane and size to have proper view of the environment on camera
            newCam.nearClipPlane = -25f;
            newCam.orthographicSize = 7.5f;

            // select only the culling mask related to the player index (layer mask positions shown in Unity Inspector)
            var camMask = newCam.cullingMask;
            switch (idx)
            {
                case 0:
                    camMask = camMask & ~(1 << 12);
                    camMask = camMask & ~(1 << 13);
                    camMask = camMask & ~(1 << 14);
                    newCam.cullingMask = camMask;
                    break;
                case 1:
                    camMask = camMask & ~(1 << 11);
                    camMask = camMask & ~(1 << 13);
                    camMask = camMask & ~(1 << 14);
                    newCam.cullingMask = camMask;
                    break;
                case 2:
                    camMask = camMask & ~(1 << 11);
                    camMask = camMask & ~(1 << 12);
                    camMask = camMask & ~(1 << 14);
                    newCam.cullingMask = camMask;
                    break;
                case 3:
                    camMask = camMask & ~(1 << 11);
                    camMask = camMask & ~(1 << 12);
                    camMask = camMask & ~(1 << 13);
                    newCam.cullingMask = camMask;
                    break;
            }

            // Instantiate GameObject with Cinemachine virtual camera to follow a specific tank
            GameObject vCamGO = new GameObject("vCam" + (idx + 1));
            CinemachineVirtualCamera vCam = vCamGO.AddComponent<CinemachineVirtualCamera>();
            vCam.Follow = m_Tanks[idx].m_Instance.transform;
            vCam.LookAt = m_Tanks[idx].m_Instance.transform;
            vCam.Priority = 20;
            
            childCam.transform.parent = m_Tanks[idx].m_Instance.transform;

            // setting camera position and rotation to show player's back
            Vector3 position = new Vector3(0f, 5f, 0f);
            Vector3 rotation = new Vector3(35f, 0f, 0f);

            childCam.transform.localPosition = position;
            childCam.transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);

            // Calculate split screen space for the new camera
            if (m_CameraControl.getSplitMode())
            {
                if (m_ActualPlayersNum > 2)
                {
                    SplitScreenFourTimes(idx, newCam);
                } else
                {
                    SplitScreenTwoTimes(idx, newCam);
                }
            } else
            {
                newCam.enabled = false;
            }
        }

        // Splits screen in half horizontally and assigns camera to one of the spaces
        // depending on idx
        private static void SplitScreenTwoTimes(int idx, Camera newCam)
        {
            if (idx == 0) newCam.rect = new Rect(0.0f, 0.5f, 1.0f, 0.5f);
            else newCam.rect = new Rect(0.0f, 0.0f, 1.0f, 0.5f);
        }

        // Splits screen horizontally and vertically and assigns camera to one of the 4
        // spaces depending on idx. If idx == 2 (meaning 3rd player) checks number of players
        // to enable minimap camera as 4th screen division
        private void SplitScreenFourTimes(int idx, Camera newCam)
        {
            switch (idx)
            {
                case 0:
                    newCam.rect = new Rect(0.0f, 0.5f, 0.5f, 0.5f);
                    break;
                case 1:
                    newCam.rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
                    break;
                case 2:
                    newCam.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);

                    // activates mainCamera to be shown in the last piece of screen
                    if (m_ActualPlayersNum == 3)
                    {
                        ShowWholeFieldVision();
                    }
                    break;
                case 3:
                    newCam.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
                    break;
            }
        }

        // Gets minimap camera GO and creates and sets its Cinemachine virtual camera
        private void ShowWholeFieldVision()
        {
            GameObject worldCamera = GameObject.Find("WorldCamera");

            Camera camera = worldCamera.GetComponent<Camera>();
            
            if (worldCamera.GetComponent<Camera>() == null)
            { 
                camera = worldCamera.AddComponent<Camera>();
                camera.CopyFrom(m_MainCamera);
            }

            // sets 4th player camera space and size
            camera.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
            camera.orthographicSize = 25f;

            // sets target and lookat to CinemachineTargetGroup
            GameObject vWorldCamera = new GameObject("vWorldCamera");
            CinemachineVirtualCamera vCamera = vWorldCamera.AddComponent<CinemachineVirtualCamera>();
            GameObject tanksTargetGroup = GameObject.Find("TanksTargetGroup");
            vCamera.Follow = tanksTargetGroup.transform;
            vCamera.LookAt = tanksTargetGroup.transform;

            // disables the camera in case the split camera is disabled when adding the
            // 3rd player
            if (!m_CameraControl.getSplitMode())
            {
                camera.enabled = false;
            }
        }

        // makes refresh of the split screen cameras
        private void RefreshSplitScreenCameras()
        {
            for (int i = 0; i < m_ActualPlayersNum; i++)
            {
                Camera cam = m_Tanks[i].m_Instance.GetComponentInChildren<Camera>();
                if (m_ActualPlayersNum == 2)
                {
                    SplitScreenTwoTimes(i, cam);
                } else if (m_ActualPlayersNum > 2)
                {
                    SplitScreenFourTimes(i, cam);
                }
            }
        }

        // Spawns tanks in the begining of the game depending on the number
        // of players selected
        private void SpawnAllTanks(List<bool> playersAreBlue)
        {
            m_MainCamera = Camera.main.GetComponent<Camera>();

            // For all the tanks...
            for (int i = 0; i < m_ActualPlayersNum; i++)
            {
                Transform spawnPoint = GetSpawnPointFromAvailableList();
                
                Vector3 spawnPointPosition = spawnPoint.position;
                Quaternion spawnPointRotation = spawnPoint.rotation;
                
                // ... create them, set their player number and references needed for control.
                m_Tanks[i].m_Instance =
                    // Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
                    Instantiate(m_TankPrefab, spawnPointPosition, spawnPointRotation) as GameObject;

                m_Tanks[i].m_SpawnPoint = spawnPoint;
                
                // Instantiates game object with the components to use new Unity Input System
                m_Tanks[i].m_MultiplayerEventSystem =
                    // Instantiate(m_MultiplayerEventSystemPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
                    Instantiate(m_MultiplayerEventSystemPrefab, spawnPointPosition, spawnPointRotation) as GameObject;
                m_Tanks[i].m_PlayerNumber = i + 1;

                if (isTeamsGame) {
                    m_Tanks[i].Setup(playersAreBlue[i] ? GameTeam.Blue : GameTeam.Red);
                } else {
                    m_Tanks[i].Setup(GameTeam.NoTeam);
                }

                AddCamera(i, m_MainCamera);
            }
        }

        private Transform GetSpawnPointFromAvailableList()
        {
            int idx = UnityEngine.Random.Range(0, m_AvailableSpawnPoints.Length);
            bool inUse = m_SpawnPointsInUse[idx];

            while (inUse)
            {
                idx++;
                if (idx >= m_AvailableSpawnPoints.Length) idx = 0;
                inUse = m_SpawnPointsInUse[idx];
            }

            m_SpawnPointsInUse[idx] = true;
            return m_AvailableSpawnPoints[idx];
        }

        // Spawns new tank when game is running
        private void SpawnNewPlayerTank()
        {
            Transform spawnPoint = GetSpawnPointFromAvailableList();
                
            Vector3 spawnPointPosition = spawnPoint.position;
            Quaternion spawnPointRotation = spawnPoint.rotation;

            int i = m_ActualPlayersNum;

            // creates the tank
            m_Tanks[i].m_Instance =
                    // Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
                    Instantiate(m_TankPrefab, spawnPointPosition, spawnPointRotation) as GameObject;

            m_Tanks[i].m_SpawnPoint = spawnPoint;

            // creates the input system prefab
            m_Tanks[i].m_MultiplayerEventSystem =
                // Instantiate(m_MultiplayerEventSystemPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
                Instantiate(m_MultiplayerEventSystemPrefab, spawnPointPosition, spawnPointRotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].PlayerName = "Player "+ m_Tanks[i].m_PlayerNumber;
            m_Tanks[i].Setup(GameTeam.NoTeam);
            AddCamera(i, m_MainCamera);

            // updates transform array in cameraControl with the player transforms
            SetNewPlayerTankAsTarget();

            m_ActualPlayersNum++;

            // updates the targets the world cam or minimap cam need to follow (CinemachineTargetGroup)
            m_CameraControl.SetActualTargetNumber(m_ActualPlayersNum);
        }

        // sets cameraControl targets (transform array with player transforms)
        // called when starting the game
        private void SetCameraTargets()
        {
            // Create a collection of transforms the same size as the number of tanks.
            Transform[] targets = new Transform[m_Tanks.Length];

            // For each of these transforms...
            for (int i = 0; i < m_ActualPlayersNum; i++)
            {
                // ... set it to the appropriate tank transform.
                targets[i] = m_Tanks[i].m_Instance.transform;
            }

            // These are the targets the camera should follow.
            m_CameraControl.m_Targets = targets;
        }

        // Adds new player to targets array in cameraControl
        private void SetNewPlayerTankAsTarget()
        {
            Transform[] targets = m_CameraControl.m_Targets;

            int i = m_ActualPlayersNum;
            targets[i] = m_Tanks[i].m_Instance.transform;
            m_CameraControl.m_Targets = targets;
        }


        // This is called from start and will run each phase of the game one after another.
        private IEnumerator GameLoop ()
        {
            // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
            yield return StartCoroutine (RoundStarting ());

            // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
            yield return StartCoroutine (RoundPlaying());

            // Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished.
            yield return StartCoroutine (RoundEnding());

            // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found.
            if (m_GameWinner != null)
            {
                // If there is a game winner, restart the level.
                SceneManager.LoadScene (0);
            }
            else
            {
                // If there isn't a winner yet, restart this coroutine so the loop continues.
                // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
                StartCoroutine (GameLoop ());
            }
        }


        private IEnumerator RoundStarting ()
        {
            // As soon as the round starts reset the tanks and make sure they can't move.
            ResetAllTanks ();
            DisableTankControl ();

            // Increment the round number and display text showing the players what round it is.
            m_RoundNumber++;
            m_MessageText.text = "ROUND " + m_RoundNumber;

            // Wait for the specified length of time until yielding control back to the game loop.
            yield return m_StartWait;
        }


        private IEnumerator RoundPlaying ()
        {
            // As soon as the round begins playing let the players control the tanks.
            EnableTankControl ();

            // Clear the text from the screen.
            m_MessageText.text = string.Empty;

            // While there is not one tank left...
            while (!OneTankLeft())
            {
                // ... return on the next frame.
                yield return null;
            }
        }


        private IEnumerator RoundEnding ()
        {
            // Stop tanks from moving.
            DisableTankControl ();

            // Clear the winner from the previous round.
            m_RoundWinner = null;

            // See if there is a winner now the round is over.
            m_RoundWinner = GetRoundWinner ();

            // If there is a winner, increment their score.
            if (m_RoundWinner != null)
                m_RoundWinner.m_Wins++;

            // Now the winner's score has been incremented, see if someone has one the game.
            m_GameWinner = GetGameWinner ();

            // Get a message based on the scores and whether or not there is a game winner and display it.
            string message = EndMessage ();
            m_MessageText.text = message;

            // Reset tanks initial position
            ResetTanksSpawnPoints();
            
            // Wait for the specified length of time until yielding control back to the game loop.
            yield return m_EndWait;
        }

        private void ResetTanksSpawnPoints()
        {
            // for (int i = 0; i < m_SpawnPointsInUse.Length; i++)
            // {
            //     m_SpawnPointsInUse[i] = false;
            // }
            m_SpawnPointsInUse = Enumerable.Repeat(false, m_AvailableSpawnPoints.Length).ToArray();
            foreach (TankManager mTank in m_Tanks)
            {
                mTank.m_SpawnPoint = GetSpawnPointFromAvailableList();
            }
        }


        // This is used to check if there is one or fewer tanks remaining and thus the round should end.
        private bool OneTankLeft()
        {
            if (!isTeamsGame) {
                // Start the count of tanks left at zero.
                int numTanksLeft = 0;
                // Go through all the tanks...
                for (int i = 0; i < m_ActualPlayersNum; i++) {
                    // ... and if they are active, increment the counter.
                    if (m_Tanks[i].m_Instance.activeSelf)
                        numTanksLeft++;
                }
                // If there are one or fewer tanks remaining return true, otherwise return false.
                return numTanksLeft <= 1;
            } else {
                bool blue = false, red = false;
                // Go through all the tanks to see if there only remains one team
                for (int i = 0; i < m_ActualPlayersNum; i++) {
                    if (m_Tanks[i].m_Instance.activeSelf) {
                        blue |= m_Tanks[i].Team == GameTeam.Blue;
                        red |= m_Tanks[i].Team == GameTeam.Red;
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
        
        
        // This function is to find out if there is a winner of the round.
        // This function is called with the assumption that 1 or fewer tanks are currently active.
        private TankManager GetRoundWinner()
        {
            // Go through all the tanks...
            for (int i = 0; i < m_ActualPlayersNum; i++)
            {
                // ... and if one of them is active, it is the winner so return it.
                if (m_Tanks[i].m_Instance.activeSelf)
                    return m_Tanks[i];
            }

            // If none of the tanks are active it is a draw so return null.
            return null;
        }


        // This function is to find out if there is a winner of the game.
        private TankManager GetGameWinner()
        {
            // Go through all the tanks...
            for (int i = 0; i < m_ActualPlayersNum; i++)
            {
                // ... and if one of them has enough rounds to win the game, return it.
                if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                    return m_Tanks[i];
            }

            // If no tanks have enough rounds to win, return null.
            return null;
        }


        // Returns a string message to display at the end of each round.
        private string EndMessage()
        {
            // By default when a round ends there are no winners so the default end message is a draw.
            string message = "DRAW!";

            // If there is a winner then change the message to reflect that.
            if (m_RoundWinner != null) {
                if (!isTeamsGame)
                    message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";
                else
                    message = "TEAM " + m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";
            }

            // Add some line breaks after the initial message.
            message += "\n\n\n\n";

            // Go through all the tanks and add each of their scores to the message.
            if (!isTeamsGame) {
                for (int i = 0; i < m_ActualPlayersNum; i++) {
                    message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
                }
            } else {
                message += "<color=#" + ColorUtility.ToHtmlStringRGB(Color.blue) + ">BLUE</color>: " + m_BlueWins + " WINS\n";
                message += "<color=#" + ColorUtility.ToHtmlStringRGB(Color.red) + ">RED</color>: " + m_RedWins + " WINS\n";
            }

            // If there is a game winner, change the entire message to reflect that.
            if (m_GameWinner != null) {
                if (!isTeamsGame)
                    message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";
                else
                    message = "TEAM " + m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";
            }

            return message;
        }


        // This function is used to turn all the tanks back on and reset their positions and properties.
        private void ResetAllTanks()
        {
            for (int i = 0; i < m_ActualPlayersNum; i++)
            {
                m_Tanks[i].Reset();
            }
        }


        private void EnableTankControl()
        {
            for (int i = 0; i < m_ActualPlayersNum; i++)
            {
                m_Tanks[i].EnableControl();
            }
        }


        private void DisableTankControl()
        {
            for (int i = 0; i < m_ActualPlayersNum; i++)
            {
                m_Tanks[i].DisableControl();
            }
        }

        // Calls Corroutine to show message of 'Max player number reached'
        private void ShowMaxPlayerReachedMessage()
        {
            StartCoroutine(ShowMaxPlayerNumMessage());
        }

        // Corroutine to show and hide message before few seconds
        IEnumerator ShowMaxPlayerNumMessage()
        {
            //Show message / bool to true
            m_MaxPlayerNumReachedMessageShowing = true;
            m_MaxPlayerNumReachedMessageGO.SetActive(true);

            yield return new WaitForSeconds(2);
            //Hidde message / bool to false
            m_MaxPlayerNumReachedMessageShowing = false;
            m_MaxPlayerNumReachedMessageGO.SetActive(false);
        }

        // Controlls event for spawning new player
        public void OnSpawnPlayer(InputAction.CallbackContext ctx)
        {
            // Debug.Log("PerformSpawn");
            if (ctx.performed && m_ActualPlayersNum < 4)
            {
                SpawnNewPlayerTank();
                RefreshSplitScreenCameras();
            }
            // max player number reached case, calls to show message method
            else if (ctx.performed && m_ActualPlayersNum >= 4)
            {
                if (!m_MaxPlayerNumReachedMessageShowing) ShowMaxPlayerReachedMessage();
            }
        }
    }
}
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
using TMPro;

namespace Offline
{
    [Serializable]
    public class TankManager
    {
        // This class is to manage various settings on a tank.
        // It works with the GameManager class to control how the tanks behave
        // and whether or not players have control of their tank in the 
        // different phases of the game.

        public Color m_PlayerColor;                             // This is the color this tank will be tinted.
        [HideInInspector] public TMP_Text m_PlayerText;                             // This is the color this tank will be tinted.
        [HideInInspector] public Transform m_SpawnPoint;                          // The position and direction the tank will have when it spawns.
        [HideInInspector] public int m_PlayerNumber;            // This specifies which player this the manager for.
        [HideInInspector] public string m_ColoredPlayerText;    // A string that represents the player with their number colored to match their tank.
        [HideInInspector] public GameObject m_Instance;         // A reference to the instance of the tank when it is created.
        [HideInInspector] public int m_Wins;                    // The number of wins this player has so far.
        [HideInInspector] public GameObject m_MultiplayerEventSystem;

        private TankMovement m_Movement;                        // Reference to tank's movement script, used to disable and enable control.
        private TankShooting m_Shooting;                        // Reference to tank's shooting script, used to disable and enable control.
        private TankHealth m_Health;
        private GameObject m_CanvasGameObject;                  // Used to disable the world space UI during the Starting and Ending phases of each round.

        private MultiplayerEventSystem m_MultiPEventSys;
        private PlayerInput m_PlayerInput;
        private GameManager m_GameManager;

        public GameManager.GameTeam Team { get; private set; }


        public void Setup (GameManager.GameTeam team)
        {
            // Get references to the components.
            m_PlayerText = m_Instance.GetComponent<TankName>().TankNameText;
            m_Movement = m_Instance.GetComponent<TankMovement>();
            m_Shooting = m_Instance.GetComponent<TankShooting>();
            m_Health = m_Instance.GetComponent<TankHealth>();
            m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas> ().gameObject;

            // Set the player numbers to be consistent across the scripts.
            m_Movement.m_PlayerNumber = m_PlayerNumber;
            m_Shooting.m_PlayerNumber = m_PlayerNumber;

            // Get all of the renderers of the tank.
            MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

            // Team
            Team = team;
            if (Team == GameManager.GameTeam.NoTeam) {
                // Create a string using the correct color that says 'PLAYER 1' etc based on the tank's color and the player's number.
                m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

                // Go through all the renderers...
                for (int i = 0; i < renderers.Length; i++) {
                    // ... set their material color to the color specific to this tank.
                    renderers[i].material.color = m_PlayerColor;
                }
            } else {
                m_PlayerColor = Team == GameManager.GameTeam.Blue ? Color.blue : Color.red;
                string t = Team == GameManager.GameTeam.Blue ? "BLUE" : "RED";
                m_Instance.tag = Team == GameManager.GameTeam.Blue ? "Blue" : "Red";
                m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">"+t+"</color>";
                // Go through all the renderers...
                for (int i = 0; i < renderers.Length; i++) {
                    // ... set their material color to the color specific to this tank.
                    renderers[i].material.color = m_PlayerColor;
                }
            }

            // Gets components for setting new Input System
            m_MultiPEventSys = m_MultiplayerEventSystem.GetComponent<MultiplayerEventSystem>();
            m_PlayerInput = m_MultiplayerEventSystem.GetComponent<PlayerInput>();

            // Sets default scheme for playerInput and uiInput module
            m_PlayerInput.defaultControlScheme = "Player" + m_PlayerNumber;
            m_PlayerInput.uiInputModule = m_MultiPEventSys.GetComponent<InputSystemUIInputModule>();
            
            // Creates one variable of type InputManager (our Input System asset script)
            // and subscribes all performed and canceled (press/release key) events to moving and shooting
            // scripts
            InputManager im = new InputManager();
            im.PlayerControlls.Movement.performed += m_Movement.OnMove;
            im.PlayerControlls.Movement.canceled += m_Movement.OnMove;
            
            im.PlayerControlls.AltShoot.performed += m_Shooting.OnAltShoot;
            im.PlayerControlls.AltShoot.canceled += m_Shooting.OnAltShoot;

            im.PlayerControlls.BombShoot.performed += m_Shooting.OnBombShoot;
            im.PlayerControlls.BombShoot.canceled += m_Shooting.OnBombShoot;

            im.PlayerControlls.Shoot.performed += m_Shooting.OnShoot;
            im.PlayerControlls.Shoot.canceled += m_Shooting.OnShoot;

            // Checking if we are setting up the 1st player tank to set the event subscription for
            // spawning new player to him. Only to player one to prevent multiple triggers at once.
            if (m_PlayerNumber == 1)
            {
                m_GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
                if (m_GameManager != null)
                {
                    im.PlayerControlls.SpawnPlayer.performed += m_GameManager.OnSpawnPlayer;
                    im.PlayerControlls.SpawnPlayer.canceled += m_GameManager.OnSpawnPlayer;
                }
            }

            // setting actions from our asset to PlayerInput and forcing scheme to default scheme and keyboard controller
            m_PlayerInput.actions = im.asset;
            m_PlayerInput.SwitchCurrentControlScheme(m_PlayerInput.defaultControlScheme, Keyboard.current);
        }

        // Used during the phases of the game where the player shouldn't be able to control their tank.
        public void DisableControl ()
        {
            m_Movement.enabled = false;
            m_Shooting.enabled = false;

            m_CanvasGameObject.SetActive (false);
        }


        // Used during the phases of the game where the player should be able to control their tank.
        public void EnableControl ()
        {
            m_Movement.enabled = true;
            m_Shooting.enabled = true;

            m_CanvasGameObject.SetActive (true);
        }


        // Used at the start of each round to put the tank into it's default state.
        public void Reset ()
        {
            m_Instance.transform.position = m_SpawnPoint.position;
            m_Instance.transform.rotation = m_SpawnPoint.rotation;

            m_Instance.SetActive (false);
            m_Instance.SetActive (true);
        }
    }
}
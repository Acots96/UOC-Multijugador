using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls; // Control
using UnityEngine.UI;
using Mirror;

namespace Complete
{
    public class TankShooting : NetworkBehaviour
    {
        public int m_PlayerNumber = 1;              // Used to identify the different players
        public Transform m_FireTransform;           // A child of the tank where the shells are spawned
        public Slider m_AimSlider;                  // A child of the tank that displays the current launch force
        public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source
        public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up
        public AudioClip m_FireClip;                // Audio that plays when each shot is fired
        public float m_MinLaunchForce = 15f;        // The force given to the shell if the fire button is not held
        public float m_MaxLaunchForce = 30f;        // The force given to the shell if the fire button is held for the max charge time
        public float m_MaxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force
        private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
        private bool m_Fired;                       // Whether or not the shell has been launched with this button press.
        public float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released
        private InputAction m_FireAction;           // Fire Action reference (Unity 2020 New Input System)
        private bool isDisabled = false;            // To avoid enabling / disabling Input System when tank is destroyed

        private ButtonControl fireControl;
        private ButtonControl fastFireControl;
        private ButtonControl bombFireControl;

        public GameObject m_ShellGO;
        public GameObject m_AltShellGO;
        public GameObject m_BombGO;

        private void OnEnable()
        {
            // When the tank is turned on, reset the launch force and the UI
            m_CurrentLaunchForce = m_MinLaunchForce;
            m_AimSlider.value = m_MinLaunchForce;

            isDisabled = false;
        }

        private void OnDisable()
        {
            isDisabled = true;
        }

        private void Start()
        {
            // Unity 2020 New Input System
            // Get a reference to the EventSystem for this player
            EventSystem ev = GameObject.Find("EventSystem").GetComponent<EventSystem>();

            // Find the Action Map for the Tank actions and enable it
            InputActionMap playerActionMap = ev.GetComponent<PlayerInput>().actions.FindActionMap("Tank");
            playerActionMap.Enable();

            // Find the 'Fire' action
            m_FireAction = playerActionMap.FindAction("Fire");
            m_FireAction.Enable();

            //m_FireAction.performed += OnFire;
            fireControl = (ButtonControl)m_FireAction.controls[0];          //Control [Barra espaciadora] para proyectil normal
            fastFireControl = (ButtonControl)m_FireAction.controls[1];      //Control [Alt Izquierdo] para dispara proyectil acelerado
            bombFireControl = (ButtonControl)m_FireAction.controls[2];      //Control [Ctrl Izquierda] para disparar proyectil bomba

            m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
        }


        private void Update()
        {
            if (!isLocalPlayer) return;
            // The slider should have a default value of the minimum launch force.
            m_AimSlider.value = m_MinLaunchForce;

            // If the max force has been exceeded and the shell hasn't yet been launched...
            if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
            {
                // ... use the max force and launch the shell.
                m_CurrentLaunchForce = m_MaxLaunchForce;
                Fire(GetTypeOfShoot());
            }
            // Otherwise, if the fire button has just started being pressed...
            else if (GetKeyStateButtonDown())
            {
                // ... reset the fired flag and reset the launch force.
                m_Fired = false;
                m_CurrentLaunchForce = m_MinLaunchForce;

                // Change the clip to the charging clip and start it playing.
                m_ShootingAudio.clip = m_ChargingClip;
                m_ShootingAudio.Play();
            }
            // Otherwise, if the fire button is being held and the shell hasn't been launched yet...
            else if (GetKeyStateButton() && !m_Fired)
            {
                // Increment the launch force and update the slider.
                m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

                m_AimSlider.value = m_CurrentLaunchForce;
            }
            // Otherwise, if the fire button is released and the shell hasn't been launched yet...
            else if (GetKeyStateButtonUp() && !m_Fired)
            {
                // ... launch the shell.
                Fire(GetTypeOfShoot());
            }
        }
        
        //Método para establecer el tipo de arma a utilizar
        private int GetTypeOfShoot()
        {
             if (fireControl.wasPressedThisFrame || fireControl.isPressed || fireControl.wasReleasedThisFrame)
            {
                return 0;
            }
            else if (fastFireControl.wasPressedThisFrame || fastFireControl.isPressed || fastFireControl.wasReleasedThisFrame)
            {
                return 1;
            }
            else if (bombFireControl.wasPressedThisFrame || bombFireControl.isPressed || bombFireControl.wasReleasedThisFrame)
            {
                return 2;
            }
            return 0;
        }

        private bool GetKeyStateButtonDown()
        {

            //En caso de presionar alguno de los botones de disparo
            return fireControl.wasPressedThisFrame || fastFireControl.wasPressedThisFrame || bombFireControl.wasPressedThisFrame;

        }

        private bool GetKeyStateButton()
        {

            //En caso de dejar sostenido alguno de los botones de disparo
            return fireControl.isPressed || fastFireControl.isPressed || bombFireControl.isPressed;

        }

        private bool GetKeyStateButtonUp()
        {

            //En caso de soltar alguno de los botones de disparo
            return fireControl.wasReleasedThisFrame || fastFireControl.wasReleasedThisFrame || bombFireControl.wasReleasedThisFrame;

        }

        [Client]
        private void Fire(int shootType)
        {
            //Debug.Log("Client Call");
            if (!isLocalPlayer) return;

            if (!isDisabled)
            {
                // Set the fired flag so only Fire is only called once.
                m_Fired = true;
                CmdFire(shootType, m_CurrentLaunchForce); // Método para la creación del proyectil en el servidor

            }
        }

        [Command]
        private void CmdFire(int shootType, float currentLaunchForce)
        {
            //Debug.Log("Command Call");
            GameObject shell =  new GameObject();

            float velFactor = 0.0f; //Velocidad del arma

            switch (shootType)
            {
                case 0:
                    //spawn para el proyectil normal
                    shell = Instantiate(m_ShellGO, m_FireTransform.position, m_FireTransform.rotation);
                    velFactor = 1.0f;
                    break;
                case 1:

                    //spawn para el proyectil rápido
                    shell = Instantiate(m_AltShellGO, m_FireTransform.position, m_FireTransform.rotation);
                    velFactor = 1.5f;
                    break;
                case 2:

                    //spawn para la bomba
                    shell = Instantiate(m_BombGO, m_FireTransform.position, m_FireTransform.rotation);
                    velFactor = 0.5f;
                    break;
            }

            //Condicional para asegurarnos que el rigid body tenga información
            if (shell.GetComponent<Rigidbody>() != null)
            {
                // Create an instance of the shell and store a reference to it's rigidbody
                Rigidbody shellInstance = shell.GetComponent<Rigidbody>();

                //Instancia del proyectil en el servidor
                NetworkServer.Spawn(shell);

                // Set the shell's velocity to the launch force in the fire position's forward direction
                shellInstance.velocity = currentLaunchForce * m_FireTransform.forward * velFactor;
                RpcFire(shell, shellInstance.velocity);

                // Change the clip to the firing clip and play it
                m_ShootingAudio.clip = m_FireClip;
                m_ShootingAudio.Play();

                // Reset the launch force.  This is a precaution in case of missing button events
                m_CurrentLaunchForce = m_MinLaunchForce;
            }
        }

        [ClientRpc] 
        private void RpcFire(GameObject go, Vector3 vel)
        {
                go.GetComponent<Rigidbody>().velocity = vel;
        }
    }
} 
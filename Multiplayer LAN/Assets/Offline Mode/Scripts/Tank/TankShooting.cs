using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Offline
{
    public class TankShooting : MonoBehaviour
    {
        public int m_PlayerNumber = 1;              // Used to identify the different players.
        public Rigidbody m_Shell;                   // Prefab of the shell.
        public Rigidbody m_ShellAlt;                   // Prefab of the red shell.
        public Rigidbody m_Bomb;                   // Prefab of the red shell.
        public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
        public Slider m_AimSlider;                  // A child of the tank that displays the current launch force.
        public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
        public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
        public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
        public float m_MinLaunchForce = 15f;        // The force given to the shell if the fire button is not held.
        public float m_MaxLaunchForce = 30f;        // The force given to the shell if the fire button is held for the max charge time.
        public float m_MaxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force.

        public float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
        private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
        public bool m_Fired;                       // Whether or not the shell has been launched with this button press.
        public bool m_AltFire;                       // Whether or not the alt shell has been launched with this button press.
        public bool m_BombFire;                       // Whether or not the alt shell has been launched with this button press.

        private float inputShootVal;
        private float inputAltShootVal;
        private float inputBombShootVal;

        private bool shootKeyPressed = false;
        private float shootKeyPressedTime = 0f;

        private bool altShootKeyPressed = false;
        private float altShootKeyPressedTime = 0f;

        private bool bombShootKeyPressed = false;
        private float bombShootKeyPressedTime = 0f;

        public bool AllowBomb = false;
        public bool AllowRapidFire = false;

        public GameObject BombIcon;
        public GameObject ShellIcon;

        private void OnEnable()
        {
            BombIcon.SetActive(false);
            ShellIcon.SetActive(false);
            // When the tank is turned on, reset the launch force and the UI
            m_CurrentLaunchForce = m_MinLaunchForce;
            m_AimSlider.value = m_MinLaunchForce;
        }


        private void Start ()
        {
            // The rate that the launch force charges up is the range of possible forces by the max charge time.
            m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
        }


        private void Update ()
        {
            // The slider should have a default value of the minimum launch force.
            m_AimSlider.value = m_MinLaunchForce;

            // If the max force has been exceeded and the shell hasn't yet been launched...
            if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
            {
                // ... use the max force and launch the shell.
                m_CurrentLaunchForce = m_MaxLaunchForce;
                Fire();
            }
            // Otherwise, if the fire button has just started being pressed...
            else if (FireButton(0) && m_Fired)
            {
                // ... reset the fired flag and reset the launch force.
                m_Fired = false;
                m_CurrentLaunchForce = m_MinLaunchForce;

                // Change the clip to the charging clip and start it playing.
                m_ShootingAudio.clip = m_ChargingClip;
                m_ShootingAudio.Play();
            }
            // Otherwise, if the fire button is being held and the shell hasn't been launched yet...
            else if (FireButton(1) && !m_Fired)
            {
                // Increment the launch force and update the slider.
                m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
                m_AimSlider.value = m_CurrentLaunchForce;
            }
            // Otherwise, if the fire button is released and the shell hasn't been launched yet...
            else if (FireButton(2) && (shootKeyPressedTime > 0f || altShootKeyPressedTime > 0f || bombShootKeyPressedTime > 0f) && !m_Fired)
            {
                // ... launch the shell.
                Fire();
            }

           if (!AllowBomb)
            {
                bombShootKeyPressed = false;
                bombShootKeyPressedTime = 0f;
                m_BombFire = false;
            }

            if (!AllowRapidFire)
            {
                altShootKeyPressed = false;
                altShootKeyPressedTime = 0f;
                m_AltFire = false;
            }

        }
        
        // method that returns if button to shot has been pressed and sets variable to
        // show if the pressed key was altFire key
        private bool FireButton(int mode)
        {
            bool action = false;
            m_AltFire = false;
            m_BombFire = false;

            switch (mode)
            {
                case 0:
                case 1:
                    action = shootKeyPressed ? true : false;
                    m_AltFire = altShootKeyPressed ? true : false;
                    m_BombFire = bombShootKeyPressed ? true : false;
                    break;
                case 2:
                    action = (!shootKeyPressed && shootKeyPressedTime > 0f) ? true : false;
                    m_AltFire = (!altShootKeyPressed && altShootKeyPressedTime > 0f) ? true : false;
                    m_BombFire = (!bombShootKeyPressed && bombShootKeyPressedTime > 0f) ? true : false;
                    break;
            }
            return action || m_AltFire || m_BombFire;
        }

        
        private void Fire ()
        {
            // Set the fired flag so only Fire is only called once.
            m_Fired = true;
            shootKeyPressedTime = 0f;
            altShootKeyPressedTime = 0f;
            bombShootKeyPressedTime = 0f;
            Rigidbody shellInstance = new Rigidbody();
            // Create an instance of the shell and store a reference to it's rigidbody.
            /*Rigidbody shellInstance = m_AltFire ?
                Instantiate(m_ShellAlt, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody :
                Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;*/

            if (m_AltFire && !m_BombFire)
            {
                shellInstance = Instantiate(m_ShellAlt, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
            }
            else if (!m_AltFire && m_BombFire)
            {
                shellInstance = Instantiate(m_Bomb, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
            }
            else if(!m_AltFire && !m_BombFire)
            {
                shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
            }
            shellInstance.tag = tag;

            // Set the shell's velocity to the launch force in the fire position's forward direction.
            shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;
            if (m_BombFire) shellInstance.velocity *= 0.50f;
            if (m_AltFire) shellInstance.velocity *= 1.50f;

            // Change the clip to the firing clip and play it.
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play ();

            // Reset the launch force.  This is a precaution in case of missing button events.
            m_CurrentLaunchForce = m_MinLaunchForce;
        }

        // Event for the Shoot Input key
        // Note: ctx.duration  is always 0 if no interaction 'Hold' is set for the key
        // 'Press and Release' interaction have to be set before 'Hold' for proper event activation
        public void OnShoot(InputAction.CallbackContext ctx)
        {
            inputShootVal = ctx.ReadValue<float>();
            
            if (ctx.performed)
            {
                shootKeyPressed = true;
                shootKeyPressedTime = 0f;
            } else if (ctx.canceled)
            {
                shootKeyPressed = false;
                shootKeyPressedTime = (float) ctx.duration;
            }

        }

        // Event for the Alt Shoot Input key
        // Note: ctx.duration  is always 0 if no interaction 'Hold' is set for the key
        // 'Press and Release' interaction have to be set before 'Hold' for proper event activation
        public void OnAltShoot(InputAction.CallbackContext ctx)
        {
            if (AllowRapidFire)
            {
                inputAltShootVal = ctx.ReadValue<float>();
                if (ctx.performed)
                {
                    altShootKeyPressed = true;
                    altShootKeyPressedTime = 0f;
                }
                else if (ctx.canceled)
                {
                    altShootKeyPressed = false;
                    altShootKeyPressedTime = (float)ctx.duration;
                }
            }
        }

        // Event for the Bomb Shoot Input key
        // Note: ctx.duration  is always 0 if no interaction 'Hold' is set for the key
        // 'Press and Release' interaction have to be set before 'Hold' for proper event activation
        public void OnBombShoot(InputAction.CallbackContext ctx)
        {
            if (AllowBomb)
            {
                inputBombShootVal = ctx.ReadValue<float>();
                if (ctx.performed)
                {
                    bombShootKeyPressed = true;
                    bombShootKeyPressedTime = 0f;
                }
                else if (ctx.canceled)
                {
                    bombShootKeyPressed = false;
                    bombShootKeyPressedTime = (float)ctx.duration;
                }
            }
        }
    }
}
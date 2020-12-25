using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class TankHealth : NetworkBehaviour
    {
        public const float m_StartingHealth = 100f;               // The amount of health each tank starts with
        public Slider m_Slider;                             // The slider to represent how much health the tank currently has
        public Image m_FillImage;                           // The image component of the slider
        public Color m_FullHealthColor = Color.green;       // The color the health bar will be when on full health
        public Color m_ZeroHealthColor = Color.red;         // The color the health bar will be when on no health
        public GameObject m_ExplosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the tank dies


        private AudioSource m_ExplosionAudio;               // The audio source to play when the tank explodes
        private ParticleSystem m_ExplosionParticles;        // The particle system the will play when the tank is destroyed

        [SyncVar(hook = "OnChangeHealth")]
        public float m_CurrentHealth = m_StartingHealth;                      // How much health the tank currently has

        [SyncVar] private bool m_Dead;                                // Has the tank been reduced beyond zero health yet?


        public bool destroyOnDeath = false;
        
        private NetworkStartPosition[] spawnPoints;
        private PlayerSpawnerSystem playerSpawnerSystem;

        private void Awake ()
        {
            // Instantiate the explosion prefab and get a reference to the particle system on it
            m_ExplosionParticles = Instantiate (m_ExplosionPrefab).GetComponent<ParticleSystem>();

            // Get a reference to the audio source on the instantiated prefab
            m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

            // Disable the prefab so it can be activated when it's required
            m_ExplosionParticles.gameObject.SetActive (false);
        }

        private void Start()
        {
            if (isLocalPlayer)
            {
                spawnPoints = FindObjectsOfType<NetworkStartPosition>();
                SetPlayerSpawnerSystem();
            }
        }

        private void SetPlayerSpawnerSystem()
        {
            playerSpawnerSystem = GameObject.FindGameObjectWithTag("PlayerSpawner").GetComponent<PlayerSpawnerSystem>();
        }

        private Vector3 GetPlayerSpawnPoint()
        {
            if (playerSpawnerSystem == null) SetPlayerSpawnerSystem();

            Transform nextSpawnTransform = playerSpawnerSystem.GetNextSpawnTransform();
            return nextSpawnTransform.position;
        }

        private void OnEnable()
        {
            // When the tank is enabled, reset the tank's health and whether or not it's dead
            m_CurrentHealth = m_StartingHealth;
            m_Dead = false;

            // Update the health slider's value and color
            SetHealthUI();
        }

        [Command]
        public void CmdHeal(float amount)
        {
            // Reduce current health by the amount of Healing is done
            m_CurrentHealth += amount;
            if (m_CurrentHealth > m_StartingHealth)
                m_CurrentHealth = m_StartingHealth;
            // Change the UI elements appropriately
            SetHealthUI();
        }

        public void TakeDamage (float amount)
        {
            if (!isServer)
            {
                return;
            }
            if (GameManager.GetOnRound())  //Flag para desactivar el daño mientras no este durante la ronda
            {
                // Reduce current health by the amount of damage done
                m_CurrentHealth -= amount;
            }
            // Change the UI elements appropriately
            SetHealthUI();

            // If the current health is at or below zero and it has not yet been registered, call OnDeath
            if (m_CurrentHealth <= 0f && !m_Dead)
            {
                OnDeath();
            }
        }


        private void SetHealthUI()
        {
            // Set the slider's value appropriately
            m_Slider.value = m_CurrentHealth;

            // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health
            m_FillImage.color = Color.Lerp (m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
        }


        private void OnChangeHealth(float oldHealth, float newHealth)
        {
            m_CurrentHealth = newHealth;
            
            // Set the slider's value appropriately
            m_Slider.value = m_CurrentHealth;

            // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health
            m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
        }

        private void OnDeath ()
        {
            // Set the flag so that this function is only called once
            m_Dead = true;

            // Move the instantiated explosion prefab to the tank's position and turn it on
            m_ExplosionParticles.transform.position = transform.position;
            m_ExplosionParticles.gameObject.SetActive (true);

            // Play the particle system of the tank exploding
            m_ExplosionParticles.Play ();

            // Play the tank explosion sound effect
            m_ExplosionAudio.Play();

            if (destroyOnDeath)
            {
                Destroy(gameObject);
                return;
            }

            m_CurrentHealth = m_StartingHealth;

            RpcRespawn();
            m_Dead = false;

        } 

        [Command]
        void CmdChangeStatusofTank(Transform player, bool status)
        {
            GameManager.TogglePlayerTank(player, status);
        }

        
        [ClientRpc]
        public void RpcRandomPos()
        {
            Vector3 spawnPoint = GetPlayerSpawnPoint();
            transform.position = spawnPoint;
        }

        [ClientRpc]
        void RpcRespawn()
        {
            if (isLocalPlayer)
            {
                CmdChangeStatusofTank(transform, false);
            }
        }
    }
}
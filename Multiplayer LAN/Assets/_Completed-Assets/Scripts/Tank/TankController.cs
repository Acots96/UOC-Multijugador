using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class TankController : NetworkBehaviour {

     [SyncVar] public int m_Wins;                    // Variable Sincronizada para almacenar rondas ganadas

    public AudioClip HealSound;
    public AudioClip PickupSound;

    public AudioSource PowerUpAudioPlayer;

     /** metodos para indicar al GameManager que debe tener en cuenta 
     * (o dejar de tener en cuenta) este tanque.
     * 
     * Esta por "duplicado" para asegurar que funciona, ya que en
     * algunos casos daba problemas.
     */


    private void Start()
    {
        Complete.GameManager.AddTank(transform);
    }

    private void OnEnable()
    {
        Complete.GameManager.AddTank(transform);
        if (isLocalPlayer)
            CmdChangeStatusofTank(this.gameObject.transform, true);
    }

    private void OnDisable()
    {
        Complete.GameManager.RemoveTank(transform);
        if (isLocalPlayer)
            CmdChangeStatusofTank(this.gameObject.transform, false);

    }
    private void OnDestroy()
    {
        Complete.GameManager.RemoveTank(transform);
    }


    [Command]
    void CmdChangeStatusofTank(Transform player, bool status)
    {
        Complete.GameManager.TogglePlayerTank(player, status);
    }

    [Client]
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PowerUp"))
        {
            if (isLocalPlayer)
            {
                var powerUpController = other.GetComponent<PowerUpController>();
                var tankShooting = GetComponent<Complete.TankShooting>();
                if (powerUpController.PowerType == PowerUpController.PowerUpType.Bomb)
                {
                    Debug.Log("Get Bomb");
                    tankShooting.AllowBomb = true;
                    tankShooting.BombIcon.SetActive(tankShooting.AllowBomb);
                    PowerUpAudioPlayer.clip = PickupSound;
                    PowerUpAudioPlayer.Play();

                    StartCoroutine(DisablePowerUp(tankShooting, PowerUpController.PowerUpType.Bomb));
                }
                else if (powerUpController.PowerType == PowerUpController.PowerUpType.RapidShell)
                {
                    Debug.Log("Get RapidFire");
                    tankShooting.AllowRapidFire = true;
                    tankShooting.ShellIcon.SetActive(tankShooting.AllowRapidFire);
                    StartCoroutine(DisablePowerUp(tankShooting, PowerUpController.PowerUpType.RapidShell));
                    PowerUpAudioPlayer.clip = PickupSound;
                    PowerUpAudioPlayer.Play();
                }
                else if (powerUpController.PowerType == PowerUpController.PowerUpType.Health)
                {
                    Debug.Log("Get Health");
                    GetComponent<Complete.TankHealth>().Heal(powerUpController.HealAmmount);
                    PowerUpAudioPlayer.clip = HealSound;
                    PowerUpAudioPlayer.Play();
                }
                CmdDestroyPowerUp(other.gameObject);
            }
        }
    }

    IEnumerator DisablePowerUp(Complete.TankShooting tankShooting, PowerUpController.PowerUpType powerUpType)
    {
        Debug.Log("Disabling Power Up");
        yield return new WaitForSeconds(8.0f);
        if (powerUpType == PowerUpController.PowerUpType.Bomb)
        {
            Debug.Log("Bomb Disabled");
            tankShooting.AllowBomb = false;
            tankShooting.BombIcon.SetActive(tankShooting.AllowBomb);
        }
        else if (powerUpType == PowerUpController.PowerUpType.RapidShell)
        {
            Debug.Log("RapidFire Disabled");
            tankShooting.AllowRapidFire = false;
            tankShooting.ShellIcon.SetActive(tankShooting.AllowRapidFire);
        }
    }


    [Command]
    void CmdDestroyPowerUp(GameObject powerUp)
    {
        Destroy(powerUp);
    }

    //Coloring

    public void Awake() {
        renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
    }

    /**
     * Obtiene el color de cada boton y le pone el listener correspondiente
     * para que llame al metodo que cambia el color.
     */
    public override void OnStartLocalPlayer() {
        GameObject[] btns = GameObject.FindGameObjectsWithTag("PlayerColorButton");
        for (int i = 0; i < btns.Length; i++) {
            GameObject b = btns[i];
            Color c = b.GetComponent<Image>().color;
            b.GetComponent<Button>().onClick.AddListener(delegate { ColorChanged(c); });
        }
        string[] s = PlayerPrefs.GetString("SelectedColor").Split(';');
        CmdColorChanged(new Color(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2])));
        
        ///Ajuste para lo de la Cámara
        Debug.Log("Local Player In");
        if (!isServer)
        {
            Complete.GameManager.SetCameraTargets();
        }

        base.OnStartLocalPlayer();
    }


    private MeshRenderer[] renderers;

    // para sincronizar la variable con los demas clientes desde el server
    // hook=SyncPlayerColor para que llame al metodo cada vez que cambie la variable
    [SyncVar(hook = "SyncPlayerColor")]
    private Color color = Color.blue;

    private void SyncPlayerColor(Color oldColor, Color newColor) {
        color = newColor;
        foreach (MeshRenderer rend in renderers)
            rend.material.color = color;
    }

    private void ColorChanged(Color c) {
        CmdColorChanged(c);
    }

    // para que se ejecute en el server y por lo tanto pueda tenerse en cuenta
    // en los demas clientes
    [Command]
    private void CmdColorChanged(Color c) {
        SyncPlayerColor(color, c);
    }
}

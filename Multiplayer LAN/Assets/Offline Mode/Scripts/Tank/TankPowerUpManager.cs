using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankPowerUpManager : MonoBehaviour
{
    public AudioClip HealSound;
    public AudioClip PickupSound;

    public AudioSource PowerUpAudioPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
                var powerUpController = other.GetComponent<Offline.PowerUpController>();
                var tankShooting = GetComponent<Offline.TankShooting>();
                if (powerUpController.PowerType == Offline.PowerUpController.PowerUpType.Bomb)
                {
                    Debug.Log("Get Bomb");
                    tankShooting.AllowBomb = true;
                    tankShooting.BombIcon.SetActive(tankShooting.AllowBomb);
                    PowerUpAudioPlayer.clip = PickupSound;
                    PowerUpAudioPlayer.Play();

                    StartCoroutine(DisablePowerUp(tankShooting, Offline.PowerUpController.PowerUpType.Bomb));
                }
                else if (powerUpController.PowerType == Offline.PowerUpController.PowerUpType.RapidShell)
                {
                    Debug.Log("Get RapidFire");
                    tankShooting.AllowRapidFire = true;
                    tankShooting.ShellIcon.SetActive(tankShooting.AllowRapidFire);
                    StartCoroutine(DisablePowerUp(tankShooting, Offline.PowerUpController.PowerUpType.RapidShell));
                    PowerUpAudioPlayer.clip = PickupSound;
                    PowerUpAudioPlayer.Play();
                }
                else if (powerUpController.PowerType == Offline.PowerUpController.PowerUpType.Health)
                {
                    Debug.Log("Get Health");
                    GetComponent<Offline.TankHealth >().Heal(powerUpController.HealAmmount);
                    PowerUpAudioPlayer.clip = HealSound;
                    PowerUpAudioPlayer.Play();
                }
                DestroyPowerUp(other.gameObject);
            }
    }

    IEnumerator DisablePowerUp(Offline.TankShooting tankShooting, Offline.PowerUpController.PowerUpType powerUpType)
    {
        Debug.Log("Disabling Power Up");
        yield return new WaitForSeconds(8.0f);
        if (powerUpType == Offline.PowerUpController.PowerUpType.Bomb)
        {
            Debug.Log("Bomb Disabled");
            tankShooting.AllowBomb = false;
            tankShooting.BombIcon.SetActive(tankShooting.AllowBomb);
        }
        else if (powerUpType == Offline.PowerUpController.PowerUpType.RapidShell)
        {
            Debug.Log("RapidFire Disabled");
            tankShooting.AllowRapidFire = false;
            tankShooting.ShellIcon.SetActive(tankShooting.AllowRapidFire);
        }
    }

    void DestroyPowerUp(GameObject powerUp)
    {
        Destroy(powerUp);
    }
}

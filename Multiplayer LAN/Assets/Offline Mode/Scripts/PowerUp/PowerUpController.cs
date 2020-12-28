using System;
using UnityEngine;

namespace Offline
{
    public class PowerUpController : MonoBehaviour
    {
        public float RotationSpeed = 15f;

        [Serializable]
        public enum PowerUpType
        {
            Bomb, Health, RapidShell
        }
        public PowerUpType PowerType;

        public float HealAmmount = 50f;

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
        }

        private void Start()
        {
            Destroy(gameObject, 12.0f);
        }
    }

}
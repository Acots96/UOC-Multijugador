using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Offline
{
    public class TankCameraController : MonoBehaviour
    {
        private Camera m_Camera;

        public float smooth = 0.5f;
        public float limitDist = 20.0f;

        private void FixedUpdate()
        {
            if (m_Camera == null)
            {
                m_Camera = GetComponentInChildren<Camera>();
                return;
            }
            Follow();
        }

        private void Follow()
        {
            float currentDist = Vector3.Distance(transform.position, m_Camera.transform.position);
            if (currentDist > limitDist)
            {
                m_Camera.transform.position = Vector3.Lerp(
                    m_Camera.transform.position, transform.position,
                    Time.deltaTime * smooth
                );
            }
        }
    }
}
using System;
using Cinemachine;
using UnityEngine;

namespace Offline
{
    public class CameraControl : MonoBehaviour
    {
        public float m_DampTime = 0.2f;                 // Approximate time for the camera to refocus.
        public float m_ScreenEdgeBuffer = 4f;           // Space between the top/bottom most target and the screen edge.
        public float m_MinSize = 6.5f;                  // The smallest orthographic size the camera can be.
        [HideInInspector] public Transform[] m_Targets; // All the targets the camera needs to encompass.


        private Camera m_WorldCamera;                        // Used for referencing the camera.

        public static bool splitMode = true;

        public float limitDist = 25.0f;
        public float hysteresis = 5.0f;

        private int m_ActualTargetNumber;
        private Camera m_MinimapCamera;

        public CinemachineVirtualCamera m_CinemachineVirtualCamera;
        public CinemachineTargetGroup m_CinemachineTargetGroup;

        public Action OnTurnOffSplitScreen;
        public Action OnTurnOnSplitScreen;


        private void Awake ()
        {
            m_WorldCamera = GetComponentInChildren<Camera> ();
        }

        private void FixedUpdate ()
        {
            Split();
        }

        // Checks if there is a pair of tanks close enough to change from split screen
        // to whole screen mode
        private void Split()
        {
            if (m_ActualTargetNumber == 0) return;
            
            if (m_MinimapCamera == null && m_ActualTargetNumber == 3)
            {
                StartWorldCamera();
            }

            float distance = GetShortestDistanceBetweenTargets();

            // Toggle to full screen
            if (splitMode && distance < (limitDist - hysteresis))
            {
                splitMode = false;

                if (m_MinimapCamera != null)
                {
                    m_MinimapCamera.enabled = false;
                }

                m_CinemachineVirtualCamera.Priority = 25;
                m_WorldCamera.rect = new Rect(0f, 0f, 1f, 1f);
                
                // Invoke caught on GameManager script
                OnTurnOffSplitScreen.Invoke();
            }
            // Toggle to split screen
            else if (!splitMode && distance > (limitDist + hysteresis))
            {
                splitMode = true;

                if (m_MinimapCamera != null)
                {
                    m_MinimapCamera.enabled = true;
                }

                m_CinemachineVirtualCamera.Priority = 20;

                // Invoke caught on GameManager script
                OnTurnOnSplitScreen.Invoke();
            }
        }

        // Get Minimap Camera reference
        private void StartWorldCamera()
        {
            m_MinimapCamera = GameObject.Find("WorldCamera").GetComponent<Camera>();
        }

        // Check all active tanks distance with other tanks to get the minimum distance
        private float GetShortestDistanceBetweenTargets()
        {
            float dist = 1000f;

            for (int i = 0; i < m_ActualTargetNumber; i++)
            {
                for (int j = 0; j < m_ActualTargetNumber; j++)
                {
                    if (i != j && (m_Targets[j].gameObject.activeSelf && m_Targets[i].gameObject.activeSelf))
                    {
                        dist = Mathf.Min(dist, Vector3.Distance(m_Targets[i].position, m_Targets[j].position));
                    }
                }
            }

            return dist;
        }

        // Updates the actual target number for the CinemachineTargetGroups component
        // adding one transform if not in the list already
        public void SetActualTargetNumber(int number)
        {
            m_ActualTargetNumber = number;
            foreach(Transform targetTransform in m_Targets)
            {
                if (targetTransform != null)
                {
                    if (m_CinemachineTargetGroup.FindMember(targetTransform) == -1)
                    {
                        m_CinemachineTargetGroup.AddMember(targetTransform, 0f, 35f);
                    }
                }
            }
        }

        // Returns boolean that says if game is in splitscreen mode
        public bool getSplitMode()
        {
            return splitMode;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Offline
{
    public class UiButtonManager : MonoBehaviour
    {
        public GameObject mainMenuGO;
        public Action<int> OnStartGame;

        public GameObject transitionCamera;

        public void StartGame(int playerNumber)
        {
            ToggleMainMenu();
            ToggleTransitionCamera();
            OnStartGame.Invoke(playerNumber);
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void ToggleMainMenu()
        {
            mainMenuGO.SetActive(!mainMenuGO.activeSelf);
            if (mainMenuGO.activeSelf)
            {
                ToggleTransitionCamera();
            }
        }

        private void ToggleTransitionCamera()
        {
            transitionCamera.SetActive(!transitionCamera.activeSelf);
        }
    }
}
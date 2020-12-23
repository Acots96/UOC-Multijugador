using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Offline
{
    public class UiButtonManager : MonoBehaviour
    {
        public GameObject mainMenuGO;
        public Action<int, List<bool>> OnStartGame;

        public GameObject transitionCamera;

        public List<Button> PlayersButtons;
        public Button StartGameButton;
        private List<bool> playersAreBlue;
        private Color blue, red;

        private void Awake() {
            blue = PlayersButtons[0].colors.disabledColor;
            red = PlayersButtons[2].colors.disabledColor;
            playersAreBlue = new List<bool>(4) { true, true, false, false };
        }

        public void StartGame(int playerNumber)
        {
            ToggleMainMenu();
            ToggleTransitionCamera();
            OnStartGame.Invoke(playerNumber, playersAreBlue);
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


        public void ChangeTeamToBlue(int player) {
            if (!playersAreBlue[player]) {
                playersAreBlue[player] = true;
                ColorBlock cb = PlayersButtons[player].colors;
                cb.disabledColor = blue;
                PlayersButtons[player].colors = cb;
                //
                int count = 0;
                foreach (bool b in playersAreBlue)
                    count += b ? 1 : -1;
                StartGameButton.interactable = count == 0;
            }
        }
        public void ChangeTeamToRed(int player) {
            if (playersAreBlue[player]) {
                playersAreBlue[player] = false;
                ColorBlock cb = PlayersButtons[player].colors;
                cb.disabledColor = red;
                PlayersButtons[player].colors = cb;
                //
                int count = 0;
                foreach (bool b in playersAreBlue)
                    count += b ? 1 : -1;
                StartGameButton.interactable = count == 0;
            }
        }
    }
}

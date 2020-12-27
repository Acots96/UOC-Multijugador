using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OfflineOnlineMenuManager : MonoBehaviour {

    public void StartLoginScene() {
        SceneManager.LoadScene("Login");
    }

    public void StartOfflineScene(int isTeams) {
        PlayerPrefs.SetInt("IsTeamsGame", isTeams);
        SceneManager.LoadScene("OfflineScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    
}

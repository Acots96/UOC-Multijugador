using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayfabController : MonoBehaviour
{

    public string userEmail;
    public string userPassword;
    public string userName;
    public string userDisplayName;

    public static PlayfabController _instance;

    private void OnEnable()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = "F30D0";
        }
        
        if (PlayerPrefs.HasKey("EMAIL"))
        {
            userEmail = PlayerPrefs.GetString("EMAIL");
            userPassword = PlayerPrefs.GetString("PASSWORD");
            userName = PlayerPrefs.GetString("USERNAME");
            
            OnClickLogin();
        }
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log(result);
        SavePlayerPrefs();

        var request = new GetPlayerProfileRequest();
        PlayFabClientAPI.GetPlayerProfile(request, OnGetPlayerProfileSuccess, OnGetPlayerProfileFailure);
        
        GoToLobby();
    }

    private static void GoToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.Log("User: " + userEmail + " does not exist. Registering new player...");
        // TODO show message about registering new user
        var registerRequest = new RegisterPlayFabUserRequest{ Email = userEmail, Password = userPassword, Username = userName };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Congratulations, new user has been registered");
        //TODO show message of success and maybe give options
        SavePlayerPrefs();
    }
    
    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
        // TODO show message about registering new user
    }

    private void OnGetPlayerProfileSuccess(GetPlayerProfileResult result)
    {
        if (result.PlayerProfile.DisplayName == null)
        {
            var request = new UpdateUserTitleDisplayNameRequest() { DisplayName = userName };
            PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnUpdateUserTitleDisplayNameSuccess, OnUpdateUserTitleDisplayNameFailure);
        }
        
        userDisplayName = result.PlayerProfile.DisplayName;
        Debug.Log(userDisplayName);
    }

    private void OnGetPlayerProfileFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    private void OnUpdateUserTitleDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log(result.DisplayName);
    }

    private void OnUpdateUserTitleDisplayNameFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
    
    private void SavePlayerPrefs()
    {
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        PlayerPrefs.SetString("USERNAME", userName);
        Debug.Log("Storing " + userEmail + " credentials into PlayerPreferences");
    }
    
    public void GetUserEmail(string userEmail)
    {
        this.userEmail = userEmail;
    }

    public void GetUserPassword(string userPassword)
    {
        this.userPassword = userPassword;
    }

    public void GetUserName(string userName)
    {
        this.userName = userName;
    }
    
    public void OnClickLogin()
    {
        var request = new LoginWithEmailAddressRequest() { Email = userEmail, Password = userPassword};
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    public void PerformLogout()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Login");
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayfabController : MonoBehaviour
{

    public string userEmail;
    public string userPassword;
    public string userName;
    public string userDisplayName;

    public static PlayfabController _instance;

    public MessageManager messageManager;
    
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
    }

    private void OnLoginFailure(PlayFabError error)
    {
        // '' invalid password or email format -> Error: InvalidParams
        // 'User not found' if email not registered -> Error: AccountNotFound
        // 'Invalid email address or password' -> Error: InvalidEmailOrPassword

        string message = "";
        
        switch (error.Error)
        {
            case PlayFabErrorCode.InvalidParams:
                message = error.GenerateErrorReport();
                messageManager.ShowMessage(true, message);
                break;
            case PlayFabErrorCode.AccountNotFound:
                message = error.GenerateErrorReport() + "\nUser: " + userEmail + " does not exist. Registering new player...";
                messageManager.ShowMessage(true, message);
                
                var registerRequest = new RegisterPlayFabUserRequest{ Email = userEmail, Password = userPassword, Username = userName };
                PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
                break;
            case PlayFabErrorCode.InvalidEmailOrPassword:
                message = error.GenerateErrorReport();
                messageManager.ShowMessage(true, message);
                break;
        }
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        string message = "Congratulations, new user has been registered";
        messageManager.ShowMessage(true, message);
        SavePlayerPrefs();
    }
    
    private void OnRegisterFailure(PlayFabError error)
    {
        string message = error.GenerateErrorReport();
        messageManager.ShowMessage(true, message);
    }

    private void OnGetPlayerProfileSuccess(GetPlayerProfileResult result)
    {
        if (result.PlayerProfile.DisplayName == null)
        {
            PerformUpdateDisplayName(userName);
        }
        
        userDisplayName = result.PlayerProfile.DisplayName;

        GoToLobby();
    }
   
    private void PerformUpdateDisplayName(string displayName)
    {
        var request = new UpdateUserTitleDisplayNameRequest() {DisplayName = displayName};
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnUpdateUserTitleDisplayNameSuccess, OnUpdateUserTitleDisplayNameFailure);
    }

    private void OnGetPlayerProfileFailure(PlayFabError error)
    {
        string message = error.GenerateErrorReport();
        messageManager.ShowMessage(true, message);
    }

    private void OnUpdateUserTitleDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("displayName updated " + result.DisplayName);
    }

    private void OnUpdateUserTitleDisplayNameFailure(PlayFabError error)
    {
        string message = error.GenerateErrorReport();
        messageManager.ShowMessage(true, message);
    }
     
    private static void GoToLobby()
    {
        _instance.GetStatistics();
        SceneManager.LoadScene("Lobby");
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

        userName = string.Empty;
        userEmail = string.Empty;
        userPassword = string.Empty;
        
        SceneManager.LoadScene("Login");
    }

    public void UpdateDisplayName(string displayName)
    {
        PerformUpdateDisplayName(displayName);
    }

    public int PlayerScore=0;

    #region Stats
    public void SetStats()
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
            Statistics = new List<StatisticUpdate> {
            new StatisticUpdate { StatisticName = "PlayerHighScore", Value = PlayerScore + 100 },
            }
        },
        result => { Debug.Log("User statistics updated"); },
        error => { Debug.LogError(error.GenerateErrorReport()); });
    }

    void GetStatistics()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStatistics,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    void OnGetStatistics(GetPlayerStatisticsResult result)
    {
        Debug.Log("Received the following Statistics:");
        foreach (var eachStat in result.Statistics)
        {
            Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
            if(eachStat.StatisticName == "PlayerHighScore")
            {
                PlayerScore = eachStat.Value;
            }
        }
    }
    #endregion

    public GameObject Leaderboard;
    public GameObject LeaderboardItemPrefab;
    public Transform LeaderboardContainer;

    #region Leaderboard
    public void GetLeaderboard()
    {
        var requestLeaderBoard = new GetLeaderboardRequest { StartPosition = 0, StatisticName = "PlayerHighScore", MaxResultsCount = 10 };
        PlayFabClientAPI.GetLeaderboard(requestLeaderBoard, OnGetLeaderboard, OnErrorLeaderboard);
    }

    void OnGetLeaderboard(GetLeaderboardResult result)
    {
        Leaderboard.SetActive(true);
        if (result.Leaderboard.Count >= 1)
        {
            //Debug.Log(result.Leaderboard[0].StatValue);
            foreach (PlayerLeaderboardEntry player in result.Leaderboard)
            {
                GameObject leaderboardItem = Instantiate(LeaderboardItemPrefab, LeaderboardContainer);
                LeaderboardManager item = leaderboardItem.GetComponent<LeaderboardManager>();

                item.PlayerNameText.text = player.DisplayName;
                item.PlayerScoreText.text = player.StatValue.ToString();
                Debug.Log(player.DisplayName + ": " + player.StatValue);
            }
        }
    }

    public void CloseLeaderboard()
    {
        Leaderboard.SetActive(false);
        for(int i = LeaderboardContainer.childCount -1; i>=0; i--)
        {
            Destroy(LeaderboardContainer.GetChild(i).gameObject);
        }
    }

    void OnErrorLeaderboard(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    #endregion
    
    #region FriendList
    
    [HideInInspector] public GameObject friendList;
    [HideInInspector] public GameObject friendListItemPrefab;
    [HideInInspector] public Transform friendListContainer;
    
    List<FriendInfo> _friends = null;

    void DisplayFriends(List<FriendInfo> friendsCache)
    {
        friendsCache.ForEach(f => Debug.Log(f.FriendPlayFabId));
    }

    void DisplayFriendListPlayFabError(PlayFabError error)
    {
        string message = error.GenerateErrorReport();
        messageManager.ShowMessage(true, message);
    }

    public void GetFriends() {
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest {
            // IncludeSteamFriends = false,
            // IncludeFacebookFriends = false,
            // XboxToken = null
        }, OnGetFriendListSuccess, DisplayFriendListPlayFabError);
    }

    private void OnGetFriendListSuccess(GetFriendsListResult result)
    {
        _friends = result.Friends;
        // DisplayFriends(_friends); // triggers your UI
        if (_friends.Count >= 1)
        {
            foreach (FriendInfo friend in _friends)
            {
                GameObject friendListItem = Instantiate(friendListItemPrefab, friendListContainer);
                FriendListItemManager item = friendListItem.GetComponent<FriendListItemManager>();

                item.SetListItemValue(friend);
            }
        }
    }

    public void AddFriend(FriendIdType idType, string friendId) {
        Debug.Log("Enters There");
        var request = new AddFriendRequest();
        switch (idType) {
            case FriendIdType.PlayFabId:
                request.FriendPlayFabId = friendId;
                break;
            case FriendIdType.Username:
                request.FriendUsername = friendId;
                break;
            case FriendIdType.Email:
                request.FriendEmail = friendId;
                break;
            case FriendIdType.DisplayName:
                request.FriendTitleDisplayName = friendId;
                break;
        }
        // Execute request and update friends when we are done
        PlayFabClientAPI.AddFriend(request, result => {
            Debug.Log("Friend added successfully!");
        }, DisplayFriendListPlayFabError);
    }
    
    // unlike AddFriend, RemoveFriend only takes a PlayFab ID
    // you can get this from the FriendInfo object under FriendPlayFabId
    public void RemoveFriend(FriendInfo friendInfo) {
        PlayFabClientAPI.RemoveFriend(new RemoveFriendRequest {
            FriendPlayFabId = friendInfo.FriendPlayFabId
        }, result => {
            _friends.Remove(friendInfo);
        }, DisplayFriendListPlayFabError);
    }
    
    #endregion
}

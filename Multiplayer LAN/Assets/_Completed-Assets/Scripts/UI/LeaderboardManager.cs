using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    public TMP_Text PlayerNameText;
    public TMP_Text PlayerScoreText;
    private PlayfabController playfabController;

    public GameObject Leaderboard;
    public GameObject LeaderboardItemPrefab;
    public Transform LeaderboardContainer;

    public bool ReferenceObjects = false;

    private void Start()
    {
        playfabController = PlayfabController._instance;
        if (ReferenceObjects)
        {
            playfabController.Leaderboard = Leaderboard;
            playfabController.LeaderboardContainer = LeaderboardContainer;
            playfabController.LeaderboardItemPrefab = LeaderboardItemPrefab;
        }
    }

    public void OnLeaderBoard()
    {
        playfabController.GetLeaderboard();
    }

    public void CloseLeaderBoard()
    {
        playfabController.CloseLeaderboard();
    }
}

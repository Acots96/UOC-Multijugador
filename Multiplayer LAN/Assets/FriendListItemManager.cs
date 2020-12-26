using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class FriendListItemManager : MonoBehaviour
{
    public TMP_Text playerName;
    
    private FriendInfo _friend;
    private PlayfabController _playfabController;

    private void Start()
    {
        _playfabController = PlayfabController._instance;
    }

    public void SetListItemValue(FriendInfo friend)
    {
        _friend = friend;
        
        playerName.text = _friend.Username + " - " + _friend.TitleDisplayName;
    }

    public void OnRemoveFriendFromList()
    {
        _playfabController.RemoveFriend(_friend);
    }
}

using PlayFab.ClientModels;
using UnityEngine;

public class FriendListItemManager : MonoBehaviour
{
    private FriendInfo _friend;
    private PlayfabController _playfabController;

    private void Start()
    {
        _playfabController = PlayfabController._instance;
    }

    public void SetListItemValue(FriendInfo friend)
    {
        _friend = friend;
    }

    public void OnRemoveFriendFromList()
    {
        _playfabController.RemoveFriend(_friend);
    }
}

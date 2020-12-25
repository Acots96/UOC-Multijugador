using System;
using UnityEngine;
using UnityEngine.UI;

public class FriendListManager : MonoBehaviour
{
    public GameObject friendList;
    public GameObject friendListItemPrefab;
    public Transform friendListContainer;

    public Button btnAddFriend;
    
    private int _friendIdTypeValue;
    private string _friendIdValue;
    private PlayfabController _playFabController;
    
    private void Start()
    {
        _friendIdTypeValue = 0;
        _friendIdValue = "";
    }

    private void OnEnable()
    {
        if (_playFabController == null)
        {
            _playFabController = PlayfabController._instance;

            _playFabController.friendList = friendList;
            _playFabController.friendListItemPrefab = friendListItemPrefab;
            _playFabController.friendListContainer = friendListContainer;
        }
        
        _playFabController.GetFriends();
    }

    public void OnFriendIdTypeValueChanged(int value)
    {
        _friendIdTypeValue = value;
    }

    public void OnFriendIdValueChanged(string value)
    {
        _friendIdValue = value;
        btnAddFriend.interactable = !_friendIdValue.Trim().Equals("");
    }

    public void OnAddFriendToFriendList()
    {
        Debug.Log("EntersHere");
        _playFabController.AddFriend((FriendIdType) _friendIdTypeValue, _friendIdValue);
    }
}

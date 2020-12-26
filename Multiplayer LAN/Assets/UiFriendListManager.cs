using UnityEngine;

public class UiFriendListManager : MonoBehaviour
{
    public GameObject FriendListGO;
    
    public void ShowFriendListUi(bool value)
    {
        FriendListGO.SetActive(value);
    }
}
using UnityEngine;

public class UiFriendListManager : MonoBehaviour
{
    public GameObject FriendListGO;
    
    public void ShowFriendListUi(bool value)
    {
        Debug.Log("Enters " + value);
        FriendListGO.SetActive(value);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoutManager : MonoBehaviour
{
    private PlayfabController playfabController;

    private void Start()
    {
        playfabController = PlayfabController._instance;
    }

    public void OnLogout()
    {
        playfabController.PerformLogout();
    }
}

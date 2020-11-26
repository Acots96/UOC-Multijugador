using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginButtonManager : MonoBehaviour
{
    private PlayfabController playfabController;
    private Button loginButton;

    private void Awake()
    {
        loginButton = GetComponent<Button>();
    }

    private void Start()
    {
        playfabController = PlayfabController._instance;
        loginButton.onClick.AddListener(playfabController.OnClickLogin);
    }
}

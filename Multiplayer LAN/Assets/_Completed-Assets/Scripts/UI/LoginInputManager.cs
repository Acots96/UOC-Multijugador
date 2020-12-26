using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginInputManager : MonoBehaviour
{
    private PlayfabController playfabController;
    
    [SerializeField] private Button loginButton;
    [SerializeField] private TMP_InputField tmpIF_email;
    [SerializeField] private TMP_InputField tmpIF_password;
    [SerializeField] private TMP_InputField tmpIF_username;

    private void Start()
    {
        playfabController = PlayfabController._instance;
        
        loginButton.onClick.AddListener(playfabController.OnClickLogin);
        tmpIF_email.onValueChanged.AddListener(playfabController.GetUserEmail);
        tmpIF_password.onValueChanged.AddListener(playfabController.GetUserPassword);
        tmpIF_username.onValueChanged.AddListener(playfabController.GetUserName);
        
        if (PlayerPrefs.HasKey("EMAIL"))
        {
            playfabController.DoAutoLogin();
        }
    }
}

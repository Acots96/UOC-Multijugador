using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayNameManager : MonoBehaviour
{
    public TMP_InputField displayName_InputField;
    
    private PlayfabController playfabController;

    private void Start()
    {
        playfabController = PlayfabController._instance;
        displayName_InputField.text = playfabController.userDisplayName;
    }

    public void OnUpdateDisplayNameClick()
    {
        playfabController.UpdateDisplayName(displayName_InputField.text);
    }
}

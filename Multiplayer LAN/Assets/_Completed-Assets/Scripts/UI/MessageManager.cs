using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    
    public GameObject messageGO;
    public TextMeshProUGUI tmp_message;
    
    private void ToggleMessageGameObject(bool value)
    {
        messageGO.SetActive(value);
    }

    public void ShowMessage(bool active, string message)
    {
        ToggleMessageGameObject(active);
        tmp_message.text = message;
    }

    public void CloseMessage()
    {
        ToggleMessageGameObject(false);
    }
}

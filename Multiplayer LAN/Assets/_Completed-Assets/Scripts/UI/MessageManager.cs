using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    
    public GameObject messageGO;
    public TextMeshProUGUI tmp_message;
    public Image panel_BackgroundImage;

    public Color successColor;
    public Color errorColor;
    
    private void ToggleMessageGameObject(bool value)
    {
        messageGO.SetActive(value);
    }

    public void ShowMessage(bool active, string message, bool isSuccess)
    {
        ToggleMessageGameObject(active);
        tmp_message.text = message;
        panel_BackgroundImage.color = isSuccess ? successColor : errorColor;
    }

    public void CloseMessage()
    {
        ToggleMessageGameObject(false);
    }
}

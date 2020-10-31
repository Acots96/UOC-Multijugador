using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class TankNaming : NetworkBehaviour
{
    public TextMeshProUGUI tmpro_tankName;

    public const string startingName = "PLAYER";

    [SyncVar(hook = "OnChangeTankName")]
    public string currentName = startingName;

    private GameObject input_playerNameGO;
    private InputField inputField_playerName;

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            input_playerNameGO = GameObject.FindGameObjectWithTag("PlayerNameInputField");
            inputField_playerName = input_playerNameGO.GetComponent<InputField>();
            inputField_playerName.onValueChanged.AddListener(delegate { InputTextValueChanged(); });
        }

    }

    private void OnChangeTankName(string oldName, string newName)
    {
        currentName = newName;

        if (newName.Equals("")) currentName = startingName;

        // Set the slider's value appropriately
        tmpro_tankName.text = currentName;
    }

    private void InputTextValueChanged()
    {

        if (!isLocalPlayer)
        {
            return;
        }
        // Update current tank name value
        currentName = inputField_playerName.text;
    }
}

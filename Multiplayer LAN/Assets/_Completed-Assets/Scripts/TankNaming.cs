using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class TankNaming : NetworkBehaviour
{
    //public TextMeshProUGUI tmpro_tankName;

    public const string startingName = "PLAYER";

    public PlayerName playerName;

    //[SyncVar(hook = "OnChangeTankName")]
    //public string currentName = startingName;

    private GameObject input_playerNameGO;
    private InputField inputField_playerName;

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            input_playerNameGO = GameObject.FindGameObjectWithTag("PlayerNameInputField");
            inputField_playerName = input_playerNameGO.GetComponent<InputField>();
            inputField_playerName.onValueChanged.AddListener(delegate { InputTextChanged(); });
        }

    }

    //private void OnChangeTankName(string oldName, string newName)
    //{
    //    currentName = newName;

    //    if (newName.Equals("")) currentName = startingName;

    //    // Set the slider's value appropriately
    //    tmpro_tankName.text = currentName;
    //}

    private void InputTextChanged()
    {
        if (!isLocalPlayer) return;
        InputTextValueChanged(inputField_playerName.text);
    }

    private void InputTextValueChanged(string name)
    {
        Debug.Log(name);
        //if (!isLocalPlayer)
        //{
        //    return;
        //}
        // Update current tank name value
        //currentName = inputField_playerName.text;
        //if (currentName.Equals("")) currentName = startingName;

        //tmpro_tankName.text = currentName;
        playerName.SetCurrentName(name);
    }
}

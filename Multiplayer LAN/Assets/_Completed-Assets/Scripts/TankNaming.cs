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

    private GameObject input_playerNameGO;
    private InputField inputField_playerName;

    [SyncVar(hook = "SyncPlayerNameUpdate")]
    private string currentName = startingName;

    private void SyncPlayerNameUpdate(string oldName, string newName)
    {
        Debug.Log("HOOK WORKED");
        currentName = newName;

        if (newName.Equals("")) currentName = startingName;

        // SET TEXT VALUE ON DISPLAY
        tmpro_tankName.text = currentName;
    }

    [Command]
    public void CmdSetCurrentName(string currentName)
    {
        SyncPlayerNameUpdate(this.currentName, currentName);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            input_playerNameGO = GameObject.FindGameObjectWithTag("PlayerNameInputField");
            inputField_playerName = input_playerNameGO.GetComponent<InputField>();
            inputField_playerName.onValueChanged.AddListener(delegate { InputTextChanged(); });

            CmdSetCurrentName(startingName);
        }

    }

    private void InputTextChanged()
    {
        if (!isLocalPlayer) return;
        CmdSetCurrentName(inputField_playerName.text);
    }
}

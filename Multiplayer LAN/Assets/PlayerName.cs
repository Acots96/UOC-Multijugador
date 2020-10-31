using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerName : NetworkBehaviour
{
    public TextMeshProUGUI tmpro_tankName;

    private const string startingName = "PLAYER";

    [SyncVar(hook = "OnPlayerNameUpdate")]
    public string currentName = startingName;

    private void OnPlayerNameUpdate(string oldName, string newName)
    {
        currentName = newName;

        if (newName.Equals("")) currentName = startingName;

        // Set the slider's value appropriately
        tmpro_tankName.text = currentName;
    }

}

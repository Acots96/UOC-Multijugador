using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerName : NetworkBehaviour
{
    public TextMeshProUGUI tmpro_tankName;

    private const string startingName = "PLAYER";

    [SyncVar(hook = "SyncPlayerNameUpdate")]
    private string currentName;

    private void SyncPlayerNameUpdate(string oldName, string newName)
    {
        Debug.Log("HOOK WORKED");
        this.currentName = newName;

        if (newName.Equals("")) this.currentName = startingName;

        // SET TEXT VALUE ON DISPLAY
        this.tmpro_tankName.text = this.currentName;
    }

    public void SetCurrentName(string name)
    {
        SyncPlayerNameUpdate(currentName, name);
    }

    private void Start()
    {
        SetCurrentName(startingName);
    }


}

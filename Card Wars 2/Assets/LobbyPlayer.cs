using System;
using Mirror;
using TMPro;
using UnityEngine;

public class LobbyPlayer : NetworkRoomPlayer
{
    [SyncVar(hook = nameof(OnDisplayNameChanged))]
    public string displayName = "Player";

    private static int playerCounter = 0;

    public override void OnStartClient()
    {
        base.OnStartClient();
        playerCounter++;
        CmdSetDisplayName($"Player {playerCounter}");
    }

    [Command]
    private void CmdSetDisplayName(string newName)
    {
        displayName = newName;
    }

    private void OnDisplayNameChanged(string oldName, string newName)
    {
        Debug.Log($"{newName} joined the lobby");
        
        String tag = "p" + playerCounter + "text";

        // GameObject.FindWithTag(tag).GetComponent<TextMeshProUGUI>().text = newName.ToUpper();
    }
}
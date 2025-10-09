using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class Room : NetworkRoomManager
{
    [Header("Match Maker Subclass Specifics")]
    
    // store the current match ID
    public string currentMatchID;

    public override void OnStartHost()
    {
        base.OnStartHost();
        currentMatchID = GetRandomMatchID();
        Debug.Log($"Host started with Match ID: {currentMatchID}");
        
        GameObject joinCode = GameObject.FindWithTag("JoinCode");

        if (joinCode == null)
        {
            Debug.LogError("Couldn't find JoinCode Text");
            return;
        }
        
        joinCode.GetComponent<TextMeshProUGUI>().text = currentMatchID;
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        NetworkConnection conn = NetworkClient.connection;
        Debug.Log($"Client connected to server with ID: {conn.connectionId}");
    }

    private string GetRandomMatchID()
    {
        return System.Guid.NewGuid().ToString().Substring(0, 5).ToUpper(); 
        // short, unique code like "A1B2C"
    }
    
    //Create your real in-game player
    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
    {
        
        GameObject gamePlayer = Instantiate(playerPrefab); 

        return gamePlayer;
    }

}

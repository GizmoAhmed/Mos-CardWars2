using Mirror;
using UnityEngine;

public class LobbyButton : MonoBehaviour
{
    private Room match;

    void Awake()
    {
        match = FindObjectOfType<Room>();
    }

    public void CreateLobby()
    {
        Debug.Log("Creating Lobby...");
        match.StartHost();
    }

    public void JoinLobby()
    {
        Debug.Log("Joining Lobby...");
        // for now, assume localhost
        match.networkAddress = "localhost"; 
        match.StartClient();
    }
    
    public void StartGame()
    {
        if (NetworkServer.active && match != null && match.numPlayers == 2)
        {
            // server start the gameplay scene
            match.ServerChangeScene(match.GameplayScene);
        }
    }
}
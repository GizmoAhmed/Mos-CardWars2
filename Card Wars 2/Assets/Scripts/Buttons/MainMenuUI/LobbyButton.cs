using GameManagement;
using Mirror;
using UnityEngine;

public class LobbyButton : MonoBehaviour
{
    private Room room;

    void Awake()
    {
        room = FindObjectOfType<Room>();
    }

    public void CreateLobby()
    {
        Debug.Log("Creating Lobby...");
        room.StartHost();
    }

    public void JoinLobby()
    {
        Debug.Log("Joining Lobby...");
        // for now, assume localhost
        room.networkAddress = "localhost"; 
        room.StartClient();
    }
    
    public void StartGame()
    {
        if (!NetworkServer.active)
        {
            Debug.LogWarning($"Cannot start game because this client doesn't see an active server.");
            return;
        }

        if (NetworkServer.active && room != null && room.numPlayers == 2)
        {
            // server start the gameplay scene, which is GameBoardV2
            room.ServerChangeScene(room.GameplayScene);
        }
    }
}
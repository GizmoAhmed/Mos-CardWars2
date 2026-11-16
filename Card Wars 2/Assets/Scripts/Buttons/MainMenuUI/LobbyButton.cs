using GameManagement;
using Mirror;
using UnityEngine;

public class LobbyButton : MonoBehaviour
{
    private Room room;
    private LanDiscovery lan;

    void Awake()
    {
        room = FindObjectOfType<Room>();
        lan = NetworkManager.singleton != null
            ? NetworkManager.singleton.GetComponent<LanDiscovery>()
            : FindObjectOfType<LanDiscovery>();
    }

    public void CreateLobby()
    {
        room.StartHost(); // Start Server in here moves to lobby scene
        lan?.Advertise();
    }

    // joiner, automatic: find a host on the LAN and connect
    public void FindGame()
    {
        lan?.Search();
    }

    // joiner, manual fallback: typed address (localhost for ParrelSync)
    public void JoinLobby(string address)
    {
        room.networkAddress = ResolveAddress(address);
        room.StartClient();
    }

    private static string ResolveAddress(string typed)
    {
#if UNITY_EDITOR
        // a ParrelSync clone is always testing on the same machine as the host
        if (ParrelSync.ClonesManager.IsClone())
            return "localhost";
#endif

        return string.IsNullOrWhiteSpace(typed) ? "localhost" : typed.Trim();
    }

    public void StartGame()
    {
        if (!NetworkServer.active || room == null || room.numPlayers != 2) return;

        lan?.Stop();   // stop broadcasting before leaving the lobby
        room.ServerChangeScene(room.GameplayScene); // move to GameBoardV2
    }
}
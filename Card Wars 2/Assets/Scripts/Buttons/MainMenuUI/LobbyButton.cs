using GameManagement;
using Mirror;
using Networking;
using UnityEngine;

namespace Buttons.MainMenuUI
{
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

        // button in the lobby online scene
        public void OnClick_StartGame()
        {
            if (!NetworkServer.active || room == null || room.numPlayers != 2) return;

            lan?.Stop();   // stop broadcasting before leaving the lobby
            room.ServerChangeScene(room.GameplayScene); // move to GameBoardV2
        }
    
        /// <summary>
        /// Goes from online Lobby, to the offline main menu
        /// </summary>
        public void OnClick_ReturnToMainMenu()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                // Host (server + client)
                Debug.Log("Host stopping server and returning to menu...");
                NetworkManager.singleton.StopHost();
            }
            else if (NetworkClient.isConnected)
            {
                // Client only (joiner)
                Debug.Log("Client disconnecting and returning to menu...");
                NetworkManager.singleton.StopClient();
            }
            else
            {
                // Offline / already in menu
                Debug.Log("Already offline, returning to menu scene.");
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu_Offline");
            }
            
            // If the joiner presses your Return button, OnClientDisconnect also fires,
            // and they'd see "Lost connection". Clear the notice in the button after stopping:
            // DisconnectNotice.Clear();
        }
    }
}
using GameManagement;
using Mirror;
using UnityEngine;

namespace Buttons.MainMenuUI
{
    public class MainMenuButton : MonoBehaviour
    {
        public GameObject join;
        
        private Room room;
        private LanDiscovery lan;

        void Awake()
        {
            room = FindObjectOfType<Room>();
            lan = NetworkManager.singleton != null
                ? NetworkManager.singleton.GetComponent<LanDiscovery>()
                : FindObjectOfType<LanDiscovery>();
        }

        public void OnClick_OpenJoinBoard()
        {
            if (join == null)
            {
                Debug.LogError("Missing Join Field - Set in Inspector");
                return;
            }
            
            join.SetActive(true);
        }

        public void OnClick_CloseJoinBoard()
        {
            transform.parent.gameObject.SetActive(false);
        }
        
        // button with 'Host' on it in the offline scene
        public void OnClick_CreateLobby()
        {
            room.StartHost(); // Start Server in here moves to lobby scene
            lan?.Advertise();
        }

        // The button for this function is in the offline scene (on the join board)
        // joiner, automatic: find a host on the LAN and connect
        public void OnClick_FindGame()
        {
            lan?.Search();
        }
    
        // The button for this function is in the offline scene (on the join board)
        // joiner, manual fallback: typed address (localhost for ParrelSync)
        public void OnClick_JoinLobby(string address)
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

        public void OnClick_QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;   // stops Play mode
            #else
                Application.Quit();                                // closes the built game
            #endif
        }
    }
}
using Mirror;
using UnityEngine;

namespace Buttons.MainMenuUI
{
    public class MainMenuUI : MonoBehaviour
    {
        public GameObject join;

        public void OnJoinClick()
        {
            if (join == null)
            {
                Debug.LogError("Missing Join Board - Set in Inspector");
            }
            
            join.SetActive(true);
        }

        public void GoBack()
        {
            transform.parent.gameObject.SetActive(false);
        }
        
        public void OnExitClicked()
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
        }
    }
}
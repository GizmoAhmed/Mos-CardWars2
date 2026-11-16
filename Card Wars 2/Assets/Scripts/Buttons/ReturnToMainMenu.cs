using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ReturnToMainMenu : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "MainMenu_Offline";

    public void OnReturnClick()
    {
        DOTween.KillAll();   // stop any running tweens before objects are destroyed

        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();      // loads Offline Scene
        }
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();    // loads Offline Scene
        }
        else
        {
            SceneManager.LoadScene(menuSceneName);    // already offline
        }
    }
}
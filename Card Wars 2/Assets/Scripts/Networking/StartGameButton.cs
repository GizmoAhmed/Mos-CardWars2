using GameManagement;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class StartGameButton : MonoBehaviour
    {
        [SerializeField] private Button startButton;

        private Room _room;

        private void Start()
        {
            _room = NetworkManager.singleton as Room;
            
            startButton = GetComponent<Button>();
            
            bool isHost = NetworkServer.active;
            startButton.gameObject.SetActive(isHost);
        }

        private void Update()
        {
            if (!NetworkServer.active || _room == null) return;

            // host: only allow starting when both players are in the room
            startButton.interactable = _room.numPlayers == 2;
        }
    }
}
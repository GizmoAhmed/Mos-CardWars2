using System.Linq;
using System.Text;
using Mirror;
using TMPro;
using UnityEngine;

namespace GameManagement
{
    public class LobbyStatusUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI statusText;

        private static LobbyStatusUI _instance;

        private void Awake()
        {
            _instance = this;
            Draw(null);   // players who joined before this scene finished loading are picked up here
        }

        private void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }

        public static void Refresh(NetworkRoomPlayer except = null)
        {
            if (_instance != null) _instance.Draw(except);
        }

        private void Draw(NetworkRoomPlayer except)
        {
            if (statusText == null) return;

            var room = NetworkManager.singleton as NetworkRoomManager;
            if (room == null) return;

            Color color;

            var players = room.roomSlots
                .Where(p => p != null && p != except)
                .OrderBy(p => p.index)
                .ToList();

            var sb = new StringBuilder();

            for (int i = 0; i < players.Count; i++)
            {
                NetworkRoomPlayer p = players[i];

                string role = p.index == 0 ? "Host" : "Joined";
                string you  = p.isLocalPlayer ? " (you)" : "";
                string ready = p.readyToBegin ? "  ✔ ready" : "";

                sb.AppendLine($"Player {i + 1} - {role}{you}{ready}");
            }

            if (players.Count < 2)
            {
                sb.AppendLine("Waiting for opponent...");
                color = Color.grey;
            }
            else
            {
                sb.AppendLine("Both players connected");
                color = Color.green;
            }

            statusText.text = sb.ToString();
            statusText.color = color;
        }
    }
}
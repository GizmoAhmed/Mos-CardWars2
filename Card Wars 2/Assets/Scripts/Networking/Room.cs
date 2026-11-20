using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManagement
{
    public class Room : NetworkRoomManager
    {
        private bool _closing;
        
        [Header("Match Maker Subclass Specifics")]
    
        // store the current match ID
        public string currentMatchID;

        public override void OnStartHost()
        {
            base.OnStartHost();
            currentMatchID = GetRandomMatchID();
            Debug.Log($"Host started with Match ID: {currentMatchID}");
        
            /*GameObject joinCode = GameObject.FindWithTag("JoinCode");

            if (joinCode == null)
            {
                Debug.LogError("Couldn't find JoinCode Text");
                return;
            }
        
            joinCode.GetComponent<TextMeshProUGUI>().text = currentMatchID;*/
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
    
        /// <summary>
        /// Overrides base call:
        /// When the scene changes to GameplayScene,
        /// Mirror calls OnRoomServerCreateGamePlayer for each connection
        /// and swaps the room player for the game player on that same connection.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="roomPlayer"></param>
        /// <returns></returns>
        public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
        {
            GameObject gamePlayer = Instantiate(playerPrefab); 

            return gamePlayer;
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);   // keep the room manager's own cleanup

            // only react in the game itself: a joiner leaving the Lobby should leave the host waiting
            bool inGame = SceneManager.GetActiveScene().path == GameplayScene;

            // the host's own connection also "disconnects" during StopHost, so ignore that
            if (!inGame || _closing || conn == NetworkServer.localConnection) return;

            _closing = true;
            Debug.Log("Opponent left the game, returning host to the menu");
            Invoke(nameof(CloseGame), 0.5f);   // let the disconnect finish before tearing down
        }

        private void CloseGame()
        {
            DG.Tweening.DOTween.KillAll();     // same cleanup as the Return button
            StopHost();                        // loads the Offline Scene
        }

        public override void OnStopHost()
        {
            base.OnStopHost();
            _closing = false;                  // reset for the next game
        }
    }
}

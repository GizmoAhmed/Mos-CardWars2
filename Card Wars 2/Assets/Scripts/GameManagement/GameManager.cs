using System.Collections.Generic;
using CardScripts.CardData;
using Mirror;
using PlayerStuff;
using UnityEngine;

// ternary operator
// variable = (condition) ? expressionTrue :  expressionFalse;

namespace GameManagement
{
    public class GameManager : NetworkBehaviour
    {
        public TurnManager turnManager;

        public MasterDeck masterDeckDb;

        [Header("Starting Player Stats")] 
        
        [Header("Magic")]
        public int maxMagic = 6;
        public int upgradeCost = 2;
        
        [Header("Money")]
        public int money = 20;
        
        [Header("Draws")]
        public int defaultFreeDrawsPerTurn = 1;
        
        public int defaultFreeDrawChoices = 1;
        public int defaultFreeDrawOffering = 3;
        
        public int defaultPaidDrawChoices = 1;
        public int defaultPaidDrawOffering = 3;
        
        [Header("Health & Drain")]
        public int health = 30;
        public int drain = 3;
        
        [Header("Rounds")]
        public int roundsToWin = 4;
        
        [Header("Connected Players")] public NetworkConnectionToClient Player1;
        public NetworkConnectionToClient Player2;

        private PlayerStats _stats1;
        private PlayerStats _stats2;

        [Header("Modals < SET IN EDITOR >")] public GameObject discardsBoardp1;
        public GameObject discardsBoardp2;
        public GameObject gmVisibleDrawModal;

        public List<NetworkConnectionToClient> playersConnections = new List<NetworkConnectionToClient>();

        /*void Start()
        {
            // Debug.Log("<<< Starting GameManager >>>");
            BuildCardDatabase();
        }*/

        [Server]
        public override void OnStartServer()
        {
            base.OnStartServer();
            turnManager = GetComponentInChildren<TurnManager>();
            turnManager.Init(this);

            masterDeckDb = GetComponentInChildren<MasterDeck>();

            if (masterDeckDb == null)
            {
                Debug.LogError("masterDeckDb is Null on gm server start");
            }

            if (discardsBoardp1 == null || discardsBoardp2 == null)
            {
                Debug.LogError("cardBoards were not set in editor and not found");
            }

            if (gmVisibleDrawModal == null)
            {
                Debug.LogError("gmVisibleDrawModal was not set in editor and therefore not found");
            }
        }

        /// <summary>
        /// Called by the server player once lobby is full
        /// </summary>
        [Server]
        public void FullLobby()
        {
            int numberOfPlayers = NetworkServer.connections.Count;

            if (numberOfPlayers == 2) // Full lobby, let's roll
            {
                AssignPlayers();

                TileManager.Instance.MemoizeTiles();

                StartPlayerStats();

                /*if (masterDeck.Count == 0)
                {
                    Debug.LogWarning($"master deck on {gameObject.name} is empty, won't copy to players");
                }*/

                turnManager.StartGame();

                masterDeckDb.InitMasterDeck();

                Player p1 = Player1.identity.GetComponent<Player>();
                Player p2 = Player2.identity.GetComponent<Player>();

                p1.playerSide = 0;
                p2.playerSide = 1;

                // game manager knows about both players, so this makes sense
                p1.deckCollection.InitializeDeck(masterDeckDb.debugDeck);
                p2.deckCollection.InitializeDeck(masterDeckDb.debugDeck);
            }
        }

        /// <summary>
        /// set players so server can recognize them
        /// </summary>
        [Server]
        private void AssignPlayers()
        {
            foreach (var conn in NetworkServer.connections.Values)
            {
                if (Player1 is null)
                {
                    Player1 = conn;
                    turnManager.DisablePlayer(Player1, false); // disable upon arrival
                }
                else
                {
                    Player2 = conn;
                    turnManager.DisablePlayer(Player2, false); // disable upon arrival
                }

                playersConnections.Add(conn);
            }
        }

        [Server]
        private void StartPlayerStats()
        {
            _stats1 = Player1.identity.GetComponent<PlayerStats>();
            _stats2 = Player2.identity.GetComponent<PlayerStats>();

            _stats1.currentSoul = _stats1.maxSoul = maxMagic;
            _stats2.currentSoul = _stats2.maxSoul = maxMagic;

            _stats1.shards = money;
            _stats2.shards = money;

            _stats1.health = health;
            _stats2.health = health;

            _stats1.drain = drain;
            _stats2.drain = drain;

            _stats1.upgradeCost = upgradeCost;
            _stats2.upgradeCost = upgradeCost;

            _stats1.freeDrawsLeft = defaultFreeDrawsPerTurn;
            _stats2.freeDrawsLeft = defaultFreeDrawsPerTurn;
            
            _stats1.freeDrawsPerTurn = defaultFreeDrawsPerTurn;
            _stats2.freeDrawsPerTurn = defaultFreeDrawsPerTurn;

            _stats1.freeCardsChosen = defaultFreeDrawChoices;
            _stats2.freeCardsChosen = defaultFreeDrawChoices;

            _stats1.freeCardsOffered = defaultFreeDrawOffering;
            _stats2.freeCardsOffered = defaultFreeDrawOffering;

            if (_stats1.playerTotalScore == 0 || _stats2.playerTotalScore == 0)
                Debug.LogWarning(
                    $"Player {gameObject.name} has default score set to 0 in editor. GameManger can't update UI from here");

            _stats1.playerTotalScore = 0;
            _stats2.playerTotalScore = 0;

            _stats1.roundsWon = -1;
            _stats2.roundsWon = -1;

            _stats1.roundsRequired = roundsToWin;
            _stats2.roundsRequired = roundsToWin;
        }

        public void RoundWin(PlayerStats winningPlayer)
        {
            Debug.Log($"...Player {winningPlayer.netId} won this round");

            winningPlayer.roundsWon++;

            Purge();

            // reset health
            _stats1.health = health + 10; // +10 for now since we aren't purging
            _stats2.health = health + 10;
        }

        public void GameWin(NetworkConnectionToClient winner)
        {
            Debug.Log($"Player {winner.connectionId} has won the whole game !!!");
        }

        public void Purge()
        {
            Debug.Log($"Purging creatures and buildings");

            // todo find object of type, run discard on each
        }

        [ContextMenu("Increase Player 1 Choice")]
        public void IncreasePlayer1Choice()
        {
            _stats1.freeCardsChosen += 1;
        }

        [ContextMenu("Increase Player 1 Offer")]
        public void IncreasePlayer1Offer()
        {
            _stats1.freeCardsOffered += 1;
        }

        [ContextMenu("Increase Player 2 Choice")]
        public void IncreasePlayer2Choice()
        {
            _stats2.freeCardsChosen += 1;
        }

        [ContextMenu("Increase Player 2 Offer")]
        public void IncreasePlayer2Offer()
        {
            _stats2.freeCardsOffered += 1;
        }

        [ContextMenu("Increase Player 1 draws left")]
        public void IncreasePlayer1DrawsLeft()
        {
            _stats1.freeDrawsLeft += 1;
        }

        [ContextMenu("Increase Player 2 draws left")]
        public void IncreasePlayer2DrawsLeft()
        {
            _stats2.freeDrawsLeft += 1;
        }
    }
}
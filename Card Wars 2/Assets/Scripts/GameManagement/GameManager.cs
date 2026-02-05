using System.Collections.Generic;
using CardScripts;
using UnityEngine;
using Mirror;
using PlayerStuff;
using Unity.VisualScripting;

// ternary operator
// variable = (condition) ? expressionTrue :  expressionFalse;

public class GameManager : NetworkBehaviour
{
    public TurnManager turnManager;

    [Tooltip("This Deck is copied to each player when both players join")]
    public List<GameObject> masterDeck;

    [Header("Starting Stats")] public int maxMagic = 6;
    public int money = 20;
    public int defaultFreeDraws = 1;
    public int defaultDrawChoices = 1;
    public int defaultDrawOffering = 3;
    public int health = 30;
    public int drain = 3;
    public int upgradeCost = 2;

    public int roundsToWin = 4;

    [Header("Connected Players")]
    public NetworkConnectionToClient Player1;
    public NetworkConnectionToClient Player2;

    public PlayerStats stats1;
    public PlayerStats stats2;

    [Header("Card Boards < SET IN EDITOR >")] 
    public GameObject discardsBoardp1;
    public GameObject discardsBoardp2;
    public GameObject drawModal;

    public List<NetworkConnectionToClient> players = new List<NetworkConnectionToClient>();

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        turnManager = GetComponentInChildren<TurnManager>();
        turnManager.Init(this);

        if (discardsBoardp1 == null || discardsBoardp2 == null)
        {
            Debug.LogError("cardBoards were not set in editor and not found");
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
            StartPlayerStats();

            if (masterDeck.Count == 0)
            {
                Debug.LogWarning($"master deck on {gameObject.name} is empty, won't copy to players");
            }

            turnManager.StartGame();

            /*
             * 'If you want each player to get their own independent copy of the master deck,
             *  you need to clone the list instead of assigning the reference.'
             */
            Player1.identity.GetComponent<Player>().deckCollection.myDeck = new List<GameObject>(masterDeck);
            Player2.identity.GetComponent<Player>().deckCollection.myDeck = new List<GameObject>(masterDeck);
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

            players.Add(conn);
        }
    }

    [Server]
    private void StartPlayerStats()
    {
        stats1 = Player1.identity.GetComponent<PlayerStats>();
        stats2 = Player2.identity.GetComponent<PlayerStats>();

        stats1.currentMagic = stats1.maxMagic = maxMagic + 2;
        stats2.currentMagic = stats2.maxMagic = maxMagic;

        stats1.money = money;
        stats2.money = money;

        stats1.health = health;
        stats2.health = health;

        stats1.drain = drain;
        stats2.drain = drain;

        stats1.upgradeCost = upgradeCost;
        stats2.upgradeCost = upgradeCost;

        stats1.freeDrawsLeft = defaultFreeDraws;
        stats2.freeDrawsLeft = defaultFreeDraws;

        stats1.freeCardsChosen = defaultDrawChoices;
        stats2.freeCardsChosen = defaultDrawChoices;
        
        stats1.freeCardsOffered = defaultDrawOffering;
        stats2.freeCardsOffered = defaultDrawOffering;

        stats1.score = 0;
        stats2.score = 0;

        stats1.roundsWon = 0;
        stats2.roundsWon = 0;

        stats1.roundsRequired = roundsToWin;
        stats2.roundsRequired = roundsToWin;
    }

    public void RoundWin(PlayerStats winningPlayer)
    {
        Debug.Log($"...Player {winningPlayer.netId} won this round");
        
        winningPlayer.roundsWon++;
        
        Purge();

        // reset health
        stats1.health = health + 10; // +10 for now since we aren't purging
        stats2.health = health + 10;
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
        stats1.freeCardsChosen += 1;
    }
    
    [ContextMenu("Increase Player 1 Offer")]
    public void IncreasePlayer1Offer()
    {
        stats1.freeCardsOffered += 1;
    }
    
    [ContextMenu("Increase Player 2 Choice")]
    public void IncreasePlayer2Choice()
    {
        stats2.freeCardsChosen += 1;
    }
    
    [ContextMenu("Increase Player 2 Offer")]
    public void IncreasePlayer2Offer()
    {
        stats2.freeCardsOffered += 1;
    }
}
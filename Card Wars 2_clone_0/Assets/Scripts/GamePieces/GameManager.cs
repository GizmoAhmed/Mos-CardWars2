using System.Collections.Generic;
using UnityEngine;
using Mirror;

// ternary operator
// variable = (condition) ? expressionTrue :  expressionFalse;

public class GameManager : NetworkBehaviour
{
	public List<GameObject> masterDeck;
	
	/*public enum DeckType { Player, Debug }

	public DeckType deckType;

	[HideInInspector] public bool p0RecievedDeck;

	[Header("Player Decks")]
	public List<GameObject> p0Deck;
	public List<GameObject> p1Deck;*/

	/*[Header("1 : master, 2 : creatures, 3 : debug")]
	public int chooseDeck;

	[Header("Debug Decks")]
	public List<GameObject> DebugMaster;
	public List<GameObject> DebugAllCreatures;
	public List<GameObject> DebugMisc;*/

	private HashSet<NetworkConnectionToClient> readyPlayers = new HashSet<NetworkConnectionToClient>();

	[Header("Phase")]
	[SyncVar] public GamePhase currentPhase;

	[Header("0 or 1, for the player you want to go first, else = random")]
	public int choosePlayer;

	[SyncVar] public bool HostGoesFirst;

	[Header("Starting Consumables")]
	public int firstMagic = 2;
	public int firstMoney = 10;
	public int firstHealth = 50;

	public NetworkConnectionToClient Player0;
	public NetworkConnectionToClient Player1;

	// private Dictionary<GamePhase, Phase> phaseHandlers;

	[HideInInspector] public Turns turnManager;

	public enum GamePhase
	{
		Offline,
		ChooseLand,
		Attack,
		SetUp,
		End
	}

	[Server]
	public override void OnStartServer()
	{
		base.OnStartServer();

		currentPhase = GamePhase.Offline;

		Debug.Log("Server started, waiting for players...");
	}

	[Server]
	public void CheckFullLobby()
	{
		int numberOfPlayers = NetworkServer.connections.Count;

		if (numberOfPlayers == 2)
		{
			IdentitfyPlayers();

			if (masterDeck.Count == 0)
			{
				Debug.LogWarning("copying over empty master deck to players");
			}

			 /*
			 * 'If you want each player to get their own independent copy of the master deck,
			 *  you need to clone the list instead of assigning the reference.'
			  */
			Player0.identity.GetComponent<Player>().deck.myDeck = new List<GameObject>(masterDeck);
			Player1.identity.GetComponent<Player>().deck.myDeck = new List<GameObject>(masterDeck);

		}
	}

	/// <summary>
	/// set players so server can recognize them
	/// </summary>
	[Server]
	public void IdentitfyPlayers()
	{
		foreach (var conn in NetworkServer.connections.Values)
		{
			if (Player0 is null)
			{
				Player0 = conn;
			}
			else
			{
				Player1 = conn;
			}
		}

		PlayerStats stats0 = Player0.identity.GetComponent<PlayerStats>();
		PlayerStats stats1 = Player1.identity.GetComponent<PlayerStats>();
		
		stats0.currentMagic = 0;
		stats0.maxMagic = 10;

		stats1.currentMagic = 0;
		stats1.maxMagic = 20;

		stats0.money = 20;
		stats1.money = 30;
		
		stats0.draws = 3;
        stats1.draws = 4;

        stats0.score = 12;
        stats1.score = 13;
        
		stats0.health = 50;
		stats1.health = 60;

		stats0.drain = 5;
		stats1.drain = 7;
		
		stats0.roundsWon = 2;
		stats1.roundsWon = 3;

		stats0.upgradeCost = 2;
		stats1.upgradeCost = 3;
	}

	//// Ready Button Click is contextual, it works differently when clicked in different phases
	[Server]
	public void PlayerReady(NetworkConnectionToClient conn)
	{
		if (currentPhase == GamePhase.ChooseLand)
		{
			if (!readyPlayers.Contains(conn))
			{
				readyPlayers.Add(conn);

				Player thisPlayer = conn.identity.GetComponent<Player>();

				// thisPlayer.RpcEnablePlayer(false);

				// CheckAllPlayersReady();
			}
		}

		// setup means each player needs to ready up  
		else if (currentPhase == GamePhase.SetUp)
		{
			// add this connection to readyPlayers (which has been cleared by now)
			readyPlayers.Add(conn);

			if (readyPlayers.Count == 1)
			{
				// pass the player (conn) who just played...
				GetComponentInChildren<Turns>().PlayerEnabler(conn);
			}
			else
			{
				readyPlayers.Clear();
				// ChangePhase(GamePhase.SetUp, GamePhase.Attack);
			}
		}
		else 
		{
			Debug.LogError("Ready was clicked when the phase wasn't Chooseland or SetUp");
		}
	}

}

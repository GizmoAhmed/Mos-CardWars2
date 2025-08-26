using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;

// ternary operator
// variable = (condition) ? expressionTrue :  expressionFalse;

public class GameManager : NetworkBehaviour
{
	public TurnManager turnManager;
	
	[Tooltip("This Deck is copied to each player when both players join")]
	public List<GameObject> masterDeck;

	[Header("Starting Stats")]
	public int maxMagic		= 6;
	public int money		= 10;
	public int drawCost		= 1;
	public int health		= 30;
	public int drain		= 2;
	public int roundsWon	= 0;
	public int upgradeCost	= 2;
	public int score		= 0;

	[Header("Connected Players")]
	public NetworkConnectionToClient Player0;
	public NetworkConnectionToClient Player1;
	
	public List<NetworkConnectionToClient> players = new List<NetworkConnectionToClient>();

	[Server]
	public override void OnStartServer()
	{
		base.OnStartServer();
		turnManager = GetComponentInChildren<TurnManager>();
		turnManager.Init(this);
	}

	/// <summary>
	/// Called by each player as they join
	/// Once there are two players present, start the game
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
			Player0.identity.GetComponent<Player>().deck.myDeck = new List<GameObject>(masterDeck);
			Player1.identity.GetComponent<Player>().deck.myDeck = new List<GameObject>(masterDeck);

		}
	}

	/// <summary>
	/// set players so server can recognize them
	/// </summary>
	[Server]
	public void AssignPlayers()
	{
		foreach (var conn in NetworkServer.connections.Values)
		{
			if (Player0 is null)
			{
				Player0 = conn;
				turnManager.DisablePlayer(Player0, false); // disable upon arrival
			}
			else
			{
				Player1 = conn;
				turnManager.DisablePlayer(Player1, false); // disable upon arrival
			}
			
			players.Add(conn);
		}
	}

	[Server]
	private void StartPlayerStats()
	{
		PlayerStats stats0 = Player0.identity.GetComponent<PlayerStats>();
		PlayerStats stats1 = Player1.identity.GetComponent<PlayerStats>();

		stats0.currentMagic = 0;
		stats1.currentMagic = 0;
		
		stats0.maxMagic = maxMagic;
		stats1.maxMagic = maxMagic;
		
		stats0.money = money;
		stats1.money = money;
		
		stats0.health = health;
		stats1.health = health;
		
		stats0.drain = drain;
		stats1.drain = drain;
		
		stats0.roundsWon = roundsWon;
		stats1.roundsWon = roundsWon;
		
		stats0.upgradeCost = upgradeCost;
		stats1.upgradeCost = upgradeCost;
		
		stats0.drawCost = drawCost;
		stats1.drawCost = drawCost;
		
		stats0.score = 0;
		stats1.score = 0;
	}
	
}

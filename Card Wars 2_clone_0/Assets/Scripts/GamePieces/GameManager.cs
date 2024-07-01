using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
	public HashSet<NetworkConnectionToClient> readyPlayers = new HashSet<NetworkConnectionToClient>();

	[SyncVar(hook = nameof(OnPhaseChanged))] public GamePhase currentPhase;

	[SyncVar] public int currentTurn;

	[Tooltip("If true, the host goes first")]
	[SyncVar] public bool hostFirst;

	[Header("Starting Consumables")]
	[SyncVar] public int startingMagic = 2;
	[SyncVar] public int startingMoney = 12;


	public Dictionary<GamePhase, Phase> phaseHandlers;

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
		InitializePhases();
		currentPhase = GamePhase.Offline;
		Debug.Log("Server started, waiting for players...");

		hostFirst = Random.value < 0.5f;
	}

	[Server]
	private void InitializePhases()
	{
		phaseHandlers = new Dictionary<GamePhase, Phase>
		{
			{ GamePhase.ChooseLand, GetComponentInChildren<ChooseLand>() },
			{ GamePhase.SetUp, GetComponentInChildren<SetUp>() },
			{ GamePhase.Attack, GetComponentInChildren<Attack>() },
			{ GamePhase.End, GetComponentInChildren<End>() }
		};

		foreach (var phase in phaseHandlers.Values)
		{
			phase.Initialize(this);
		}
	}

	private void OnPhaseChanged(GamePhase oldPhase, GamePhase newPhase)
	{
		if (phaseHandlers == null || !phaseHandlers.ContainsKey(newPhase))
		{
			return;
		}

		if (oldPhase != GamePhase.Offline && phaseHandlers.ContainsKey(oldPhase))
		{
			phaseHandlers[oldPhase]?.OnExitPhase();
		}

		phaseHandlers[newPhase]?.OnEnterPhase();
	}

	[Server]
	public void CheckFullLobby()
	{
		int numberOfPlayers = NetworkServer.connections.Count;

		if (numberOfPlayers == 2)
		{
			Debug.Log("Let the game begin!");
			currentPhase = GamePhase.ChooseLand;

			SetUp setUpScript = GetComponentInChildren<SetUp>();

			setUpScript.IdentitfyPlayers();
		}
	}

	[Server]
	public void PlayerReady(NetworkConnectionToClient conn)
	{
		//// Ready Button Click is contextual, it works differently when clicked in different phases

		// choose land requires that both players ready up
		if (currentPhase == GamePhase.ChooseLand) 
		{
			if (!readyPlayers.Contains(conn))
			{
				readyPlayers.Add(conn);

				Player thisPlayer = conn.identity.GetComponent<Player>();

				thisPlayer.RpcEnablePlayer(false);

				Debug.Log($"Player {conn.connectionId} has chosen land");
				Debug.Log($"Disabling Player {conn.connectionId}");
				
				CheckAllPlayersReady();
			}
		}

		// setup means each player needs to ready up  
		else if (currentPhase == GamePhase.SetUp) 
		{
			Debug.Log($"Player {conn.connectionId} is done setting up...handing over control");

			// add this connection to readyPlayers (which has been cleard by now)
			readyPlayers.Add(conn);

			/*
			then check size
			if (if size 1, then other player isn't ready , this player must've set up first)
			{
				false myTurn from the player that called this, and give it to the other guy so they can set up
				use -> var player = conn.identity.GetComponent<Player>(); in order to switch the turn
			}
			
			if (if size 2, then both players finished set up)
			{
				clear readyPlayers for next set up.
				go to attack phase
			}
			 */

		}

	}

	[Server]
	private void CheckAllPlayersReady()
	{
		if (readyPlayers.Count >= 2)
		{
			foreach (var connection in readyPlayers) 
			{
				var player = connection.identity.GetComponent<Player>();
				player.RpcEnablePlayer(true);
			}

			readyPlayers.Clear();

			Debug.Log("All players ready, lets move on:");

			NextPhase();
		}
		else
		{
			Debug.Log("Waiting for the other player to ready up...");
		}
	}

	[Server]
	public void SetConsumables(int magic, int money) 
	{
		foreach (var conn in NetworkServer.connections.Values)
		{
			var player = conn.identity.GetComponent<Player>();
			player.RpcUpdateMagic(magic);
			player.RpcUpdateMoney(money);
		}
	}



	[Server]
	public void NextPhase()
	{
		currentTurn++;
		if (currentPhase == GamePhase.ChooseLand)
		{
			Debug.Log("ChooseLand -> Set Up");
			currentPhase = GamePhase.SetUp;
		}
		else if (currentPhase == GamePhase.SetUp)
		{
			Debug.Log("Set Up -> Attack");
			currentPhase = GamePhase.Attack;
		}
		else if (currentPhase == GamePhase.Attack)
		{
			Debug.Log("Attack -> Set Up");
			currentPhase = GamePhase.SetUp;
		}
	}

	[Server]
	public void IncrementTurn() 
	{
		currentTurn++;

		// update all spells and building timers here

		Debug.Log("Incrementing turns");

		foreach (var conn in NetworkServer.connections.Values)
		{
			var player = conn.identity.GetComponent<Player>();
			player.RpcUpdateTurnText(currentTurn);
		}
	}
}

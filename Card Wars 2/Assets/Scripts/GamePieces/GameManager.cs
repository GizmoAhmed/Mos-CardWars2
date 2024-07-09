using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
	public HashSet<NetworkConnectionToClient> readyPlayers = new HashSet<NetworkConnectionToClient>();

	[SyncVar] public GamePhase currentPhase;

	[SyncVar] public int currentTurn;

	[Tooltip("If true, the host goes first")]
	[SyncVar] public bool hostFirst;

	[Header("Starting Consumables")]
	[SyncVar] public int startingMagic;
	[SyncVar] public int startingMoney;


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
		hostFirst = Random.value < 0.5f;
		InitializePhases();
		currentPhase = GamePhase.Offline;
		Debug.Log("Server started, waiting for players...");
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

	[Server]
	public void ChangePhase(GamePhase oldPhase, GamePhase newPhase)
	{
		Debug.Log(	"< Phase Change Detected > \n "
					+ oldPhase.ToString() + " > " + newPhase.ToString());

		currentPhase = newPhase;

		if (phaseHandlers == null || !phaseHandlers.ContainsKey(newPhase))
		{
			return;
		}

		// exit the old phase if is not offline
		if (oldPhase != GamePhase.Offline)
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
			ChangePhase(GamePhase.Offline, GamePhase.ChooseLand);

			GetComponentInChildren<SetUp>().IdentitfyPlayers();
		}
	}

	//// Ready Button Click is contextual, it works differently when clicked in different phases
	[Server]
	public void PlayerReady(NetworkConnectionToClient conn)
	{
		if (currentPhase == GamePhase.Offline) 
		{
			Debug.LogWarning("Ready Button is clicked during offline, shouldn't be possible");
		}
		// choose land requires that both players ready up
		else if (currentPhase == GamePhase.ChooseLand)
		{
			if (!readyPlayers.Contains(conn))
			{
				readyPlayers.Add(conn);

				Player thisPlayer = conn.identity.GetComponent<Player>();

				thisPlayer.RpcEnablePlayer(false);

				Debug.Log($"Player {conn.connectionId} has chosen land");

				CheckAllPlayersReady();
			}
		}

		// setup means each player needs to ready up  
		else if (currentPhase == GamePhase.SetUp)
		{
			Debug.Log($"Player {conn.connectionId} is done setting up...handing over control");

			// add this connection to readyPlayers (which has been cleard by now)
			readyPlayers.Add(conn);

			if (readyPlayers.Count == 1)
			{
				// pass the player (conn) who just played...
				GetComponentInChildren<SetUp>().ManageTurn(conn);
			}
			else
			{
				readyPlayers.Clear();
				ChangePhase(GamePhase.SetUp, GamePhase.Attack);
			}
		}
	}

	[Server]
	private void CheckAllPlayersReady()
	{
		if (readyPlayers.Count >= 2)
		{
			/*foreach (var connection in readyPlayers) 
			{
				var player = connection.identity.GetComponent<Player>();
				player.RpcEnablePlayer(true);
			}*/

			readyPlayers.Clear();

			Debug.Log("All players ready, lets move on:");

			ChangePhase(GamePhase.ChooseLand, GamePhase.SetUp);
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
	public void IncrementTurn() 
	{
		// update all spells and building timers here mabye
		foreach (var conn in NetworkServer.connections.Values)
		{
			conn.identity.GetComponent<Player>().RpcUpdateTurnText(currentTurn++);
		}
	}
}

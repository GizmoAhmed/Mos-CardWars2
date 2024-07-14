using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
	public HashSet<NetworkConnectionToClient> readyPlayers = new HashSet<NetworkConnectionToClient>();

	[SyncVar] public GamePhase currentPhase;

	[Tooltip("Set number to the player you want to go first")] public int ChoosePlayer;
	[SyncVar] public bool HostGoesFirst;

	[Header("Starting Consumables")]
	[SyncVar] public int firstMagic = 2;
	[SyncVar] public int firstMoney = 10;

	public NetworkConnectionToClient Player0;
	public NetworkConnectionToClient Player1;

	public Dictionary<GamePhase, Phase> phaseHandlers;

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

		if (ChoosePlayer == 0)		{ HostGoesFirst = true; }

		else if (ChoosePlayer == 1)	{ HostGoesFirst = false; }

		else						{ HostGoesFirst = Random.value < 0.5f; }

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

		turnManager = GetComponentInChildren<Turns>();
		turnManager.InitializeTurns(this);
	}

	[Server]
	public void ChangePhase(GamePhase oldPhase, GamePhase newPhase)
	{
		// Debug.Log(oldPhase.ToString() + " > " + newPhase.ToString());

		currentPhase = newPhase;

		if (phaseHandlers == null || !phaseHandlers.ContainsKey(newPhase))
		{
			return;
		}

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
			// Debug.Log("Let the game begin!");
			ChangePhase(GamePhase.Offline, GamePhase.ChooseLand);

			IdentitfyPlayers();
		}
	}

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

				thisPlayer.RpcEnablePlayer(false);

				CheckAllPlayersReady();
			}
		}

		// setup means each player needs to ready up  
		else if (currentPhase == GamePhase.SetUp)
		{
			// add this connection to readyPlayers (which has been cleard by now)
			readyPlayers.Add(conn);

			if (readyPlayers.Count == 1)
			{
				// pass the player (conn) who just played...
				GetComponentInChildren<Turns>().ManageTurn(conn);
			}
			else
			{
				readyPlayers.Clear();
				ChangePhase(GamePhase.SetUp, GamePhase.Attack);
			}
		}
		else 
		{
			Debug.LogError("Ready was clicked when the phase wasn't Chooseland or SetUp");
		}
	}

	[Server]
	private void CheckAllPlayersReady()
	{
		if (readyPlayers.Count >= 2)
		{
			readyPlayers.Clear();
			ChangePhase(GamePhase.ChooseLand, GamePhase.SetUp);
		}

	}
}

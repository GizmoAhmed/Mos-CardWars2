using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
	public List<Player> playersInLobby;

	public HashSet<NetworkConnectionToClient> readyPlayers = new HashSet<NetworkConnectionToClient>();

	[SyncVar(hook = nameof(OnPhaseChanged))] public GamePhase currentPhase;

	[SyncVar] public int currentTurn;

	[SyncVar] public bool hostFirst;

	public Dictionary<GamePhase, Phase> phaseHandlers;

	public enum GamePhase
	{
		Offline,
		ChooseLand,
		Start,
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
			{ GamePhase.Start, GetComponentInChildren<Start>() },
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
		if (phaseHandlers == null)
		{
			Debug.LogWarning($"Probabily a client right? PhaseHandler not initialized");
			return;
		}

		if (!phaseHandlers.ContainsKey(newPhase)) 
		{
			Debug.LogWarning($"new phase {newPhase} is invalid.");
			return;
		}

		if (oldPhase != GamePhase.Offline && phaseHandlers.ContainsKey(oldPhase))
		{
			phaseHandlers[oldPhase]?.OnExitPhase();
		}

		phaseHandlers[newPhase]?.OnEnterPhase();
		// phaseHandlers[newPhase]?.HandlePhaseLogic();
	}

	[Server]
	public void CheckFullLobby()
	{
		int numberOfPlayers = NetworkServer.connections.Count;

		if (numberOfPlayers == 2)
		{
			Debug.Log("Let the game begin!");
			currentPhase = GamePhase.ChooseLand;
		}
	}

	[Server]
	public void PlayerReady(NetworkConnectionToClient conn)
	{
		if (!readyPlayers.Contains(conn))
		{
			readyPlayers.Add(conn);
			Debug.Log($"Player {conn.connectionId} is ready.");
			CheckAllPlayersReady();
		}
	}

	[Server]
	private void CheckAllPlayersReady()
	{
		if (readyPlayers.Count >= 2)
		{
			readyPlayers.Clear();
			Debug.Log("Both players ready, moving on.");
			NextTurn();
		}
		else
		{
			Debug.Log("Waiting for the other player...");
		}
	}

	[Server]
	public void StartingConsumables(int magic, int money) 
	{
		foreach (var conn in NetworkServer.connections.Values)
		{
			var player = conn.identity.GetComponent<Player>();
			player.RpcUpdateMagic(magic);
			player.RpcUpdateMoney(money);
		}
	}

	[Server]
	public void NextTurn()
	{
		currentTurn++;

		if (currentPhase == GamePhase.ChooseLand)
		{
			Debug.Log("Lands chosen, lets go to the next phase: setup");
			currentPhase = GamePhase.Start;
		}
		else if (currentPhase == GamePhase.Start)
		{
			Debug.Log("Start complete");
			currentPhase = GamePhase.Attack;
		}
		else
		{
			Debug.Log("Incrementing turns");

			foreach (var conn in NetworkServer.connections.Values)
			{
				var player = conn.identity.GetComponent<Player>();
				player.RpcUpdateTurnText(currentTurn);
			}
		}
	}
}

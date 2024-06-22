using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
	public HashSet<NetworkConnectionToClient> readyPlayers = new HashSet<NetworkConnectionToClient>();

	[SyncVar(hook = nameof(OnPhaseChanged))]
	public GamePhase currentPhase;

	[SyncVar] public int currentTurn;

	private Dictionary<GamePhase, Phase> phaseHandlers;

	public enum GamePhase
	{
		Offline,
		ChooseLand,
		SetUp,
		Attack,
		End
	}

	[Server]
	public override void OnStartServer()
	{
		base.OnStartServer();
		InitializePhases();
		currentPhase = GamePhase.Offline;
		Debug.Log("Server started, waiting for players...");
	}

	private void InitializePhases()
	{
		phaseHandlers = new Dictionary<GamePhase, Phase>
		{
			{ GamePhase.ChooseLand, GetComponentInChildren<ChooseLand>() },
			{ GamePhase.SetUp, GetComponentInChildren<SetUp>() },
			{ GamePhase.Attack, GetComponentInChildren<Attack>() },
			{ GamePhase.End, GetComponentInChildren<End>() }
		};

		foreach (var handler in phaseHandlers.Values)
		{
			handler.Initialize(this);
		}
	}

	private void OnPhaseChanged(GamePhase oldPhase, GamePhase newPhase)
	{
		if (phaseHandlers == null || !phaseHandlers.ContainsKey(newPhase))
		{
			Debug.LogError($"PhaseHandler not initialized or new phase {newPhase} is invalid.");
			return;
		}

		if (oldPhase != GamePhase.Offline && phaseHandlers.ContainsKey(oldPhase))
		{
			phaseHandlers[oldPhase]?.OnExitPhase();
		}

		phaseHandlers[newPhase]?.OnEnterPhase();
		phaseHandlers[newPhase]?.HandlePhaseLogic();
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
	public void NextTurn()
	{
		currentTurn++;

		foreach (var conn in NetworkServer.connections.Values)
		{
			var player = conn.identity.GetComponent<Player>();
			player.RpcUpdateTurnText(currentTurn);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
	public HashSet<NetworkConnectionToClient> readyPlayers = new HashSet<NetworkConnectionToClient>();

	// [SyncVar(hook = nameof(OnGamePhaseChanged))] public GamePhase currentPhase = GamePhase.Start;

	public enum GamePhase
	{
		Start,
		SetUp,
		Attack,
		End
	}

	[Server]
	public void PlayerReady(NetworkConnectionToClient conn)
	{
		if (!readyPlayers.Contains(conn))
		{
			readyPlayers.Add(conn);
			Debug.Log($"Player {conn.connectionId} is ready.");
			// CheckAllPlayersReady();
		}
	}

	/*
	[Server]
	private void CheckAllPlayersReady()
	{
		if (readyPlayers.Count >= 2) // Assuming a 2-player game
		{
			AdvancePhase();
			readyPlayers.Clear(); // Reset ready state for the next phase
		}
		else
		{
			Debug.Log("the other guy isn't ready...");
		}
	}

	[Server]
	private void AdvancePhase()
	{
		switch (currentPhase)
		{
			case GamePhase.Start:
				currentPhase = GamePhase.SetUp;
				break;
			case GamePhase.SetUp:
				currentPhase = GamePhase.Attack;
				break;
			case GamePhase.Attack:
				currentPhase = GamePhase.End;
				break;
			case GamePhase.End:
				currentPhase = GamePhase.Start; // Loop back to Start for simplicity
				break;
		}
	}

	private void OnGamePhaseChanged(GamePhase oldPhase, GamePhase newPhase)
	{
		Debug.Log($"Game phase updated to: {newPhase}");
		HandleGamePhaseChanged(newPhase);
	}

	private void HandleGamePhaseChanged(GamePhase newPhase)
	{
		// Handle any client-side logic for phase change here
	}
	*/
}

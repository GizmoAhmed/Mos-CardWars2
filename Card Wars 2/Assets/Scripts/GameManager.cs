using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor.Tilemaps;
using TMPro;

public class GameManager : NetworkBehaviour
{
	public HashSet<NetworkConnectionToClient> readyPlayers = new HashSet<NetworkConnectionToClient>();

	public GamePhase currentPhase;

	[SyncVar] public int currentTurn;

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
		currentPhase = GamePhase.Offline;
		Debug.Log("Server started, waiting for players...");
	}

	[Server]
	public void CheckFullLobby() 
	{
		int numberofPlayers = NetworkServer.connections.Count;

		if (numberofPlayers == 2) 
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
			Debug.Log("Both Ready, lets move on");
			NextTurn();
		}
		else
		{
			Debug.Log("the other guy isn't ready...");
		}
	}

	[Server]
	public void NextTurn()
	{
		currentTurn++;

		/// 'var' is equvialent to 'auto'

		foreach (var conn in NetworkServer.connections.Values)
		{
			var player = conn.identity.GetComponent<Player>();
			player.RpcUpdateTurnText(currentTurn);
		}
	}
}

using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
	private List<Player> players = new List<Player>();
	private int currentPlayerIndex = 0;

	[Header("Turn and Phase Management")]
	public int currentTurn = 1;
	public Phase currentPhase = Phase.Start;

	public enum Phase
	{
		Start,
		SetUp,
		Attack,
		End
	}

	public void RegisterPlayer(Player player)
	{
		if (!players.Contains(player))
		{
			players.Add(player);
		}
	}

	public void StartNextTurn()
	{
		currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
		currentTurn++;
		currentPhase = Phase.Start;

		// Notify players of the new turn
		foreach (var player in players)
		{
			player.TargetStartTurn(player.connectionToClient, currentTurn, currentPhase);
		}
	}

	[ClientRpc]
	public void RpcStartPhase(Phase newPhase)
	{
		currentPhase = newPhase;

		// Notify all players of the phase change
		foreach (var player in players)
		{
			player.TargetStartPhase(player.connectionToClient, currentPhase);
		}
	}

	[ClientRpc]
	public void RpcEndTurn()
	{
		StartNextTurn();
	}

	[Command]
	public void CmdReadyForNextPhase(Player player)
	{
		// Check if all players are ready for the next phase
		// For simplicity, we move to the next phase immediately
		Phase nextPhase = currentPhase + 1;
		if (nextPhase > Phase.End)
		{
			RpcEndTurn();
		}
		else
		{
			RpcStartPhase(nextPhase);
		}
	}
}

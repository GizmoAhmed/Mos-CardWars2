using Mirror;
using UnityEngine;

public class Attack : Phase
{
	[Server]
	public override void OnEnterPhase()
	{
		Debug.Log("Phasing Attack...Disabling both players actions");

		foreach (Player player in gameManager.playersInLobby)
		{
			player.myTurn = false;
		}
	}

	[ClientRpc]
	public override void OnExitPhase()
	{
		base.OnExitPhase();

		Debug.Log("Leaving attack...enabling A player'S action");

		foreach (Player player in gameManager.playersInLobby)
		{
			player.myTurn = true;
		}
	}

	[Server]
	public override void HandlePhaseLogic()
	{
		base.HandlePhaseLogic();
	}
}

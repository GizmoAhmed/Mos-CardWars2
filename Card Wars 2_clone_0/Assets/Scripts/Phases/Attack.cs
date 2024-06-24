using Mirror;
using UnityEngine;

 // During this phase, players are disabled, to watch the attacks go down.

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

		HandlePhaseLogic();
	}

	[Server]
	public override void OnExitPhase()
	{
		base.OnExitPhase();

		Debug.Log("Leaving attack...enabling players");

		foreach (Player player in gameManager.playersInLobby)
		{
			player.myTurn = true;
		}

		// the set up phase, the phase immediately after this one, should disable one player
	}

	[Server]
	public override void HandlePhaseLogic()
	{
		Debug.Log("Attack scene would go down right here...");

		gameManager.NextPhase();
	}
}

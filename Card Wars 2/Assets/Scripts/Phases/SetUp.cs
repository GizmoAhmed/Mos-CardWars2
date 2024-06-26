using Mirror;
using UnityEngine;

public class SetUp : Phase
{
	[Server]
	public override void OnEnterPhase()
	{
		/// disable one of the players, based on gameManager host boolean
		Debug.Log("Entering a set up phase");

		/// find a way to update the money, which +2 of whatever you had last turn
		// gameManager.SetConsumables(gameManager.startingMagic + gameManager.currentTurn, );
	}

	[Server] 
	public override void OnExitPhase()
	{
		gameManager.IncrementTurn();
	}

	[Server]
	public override void HandlePhaseLogic()
	{

	}
}

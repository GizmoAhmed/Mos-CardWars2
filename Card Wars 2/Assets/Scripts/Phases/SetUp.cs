using Mirror;
using UnityEngine;

public class SetUp : Phase
{
	[SyncVar] public int MagicAddOn;

	public override void Initialize(GameManager gameManager)
	{
		this.gameManager = gameManager;
		MagicAddOn = 0;
	}

	[Server]
	public override void OnEnterPhase()
	{
		/// disable one of the players, based on gameManager host boolean
		Debug.Log("Entering a set up phase");

		/// find a way to update the money, which +2 of whatever you had last turn
		gameManager.SetConsumables(gameManager.startingMagic + MagicAddOn, gameManager.startingMoney);

		MagicAddOn++;

		HandlePhaseLogic();
	}

	[Server] 
	public override void OnExitPhase()
	{
		// gameManager.IncrementTurn();
	}

	[Server]
	public override void HandlePhaseLogic()
	{
		Debug.Log("Set Up's phase logic...");
	}
}

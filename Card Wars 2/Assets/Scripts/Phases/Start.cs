using Mirror;
using UnityEngine;

public class Start : Phase
{	
	public override void Initialize(GameManager gameManager)
	{
		base.Initialize(gameManager);
	}

	[Server]
	public override void OnEnterPhase()
	{
		Debug.Log("Entering Start Phase...");
		gameManager.SetConsumables(gameManager.startingMagic, gameManager.startingMoney);
	}

	[ClientRpc]
	public override void OnExitPhase()
	{
		base.OnExitPhase();
	}

	[Server]
	public override void HandlePhaseLogic()
	{
		base.HandlePhaseLogic();
	}
}

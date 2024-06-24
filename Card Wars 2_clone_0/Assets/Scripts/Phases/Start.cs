using Mirror;
using UnityEngine;

public class Start : Phase
{	
	public int startingMagic = 2;
	public int startingMoney = 12;
	

	public override void Initialize(GameManager gameManager)
	{
		base.Initialize(gameManager);
	}

	[Server]
	public override void OnEnterPhase()
	{
		Debug.Log("Entering Start Phase...");
		gameManager.StartingConsumables(startingMagic, startingMoney);
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

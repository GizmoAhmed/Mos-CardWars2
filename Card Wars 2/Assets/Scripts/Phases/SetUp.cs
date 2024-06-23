using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUp : Phase
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
		Debug.Log("Entering Set Up Phase...");
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

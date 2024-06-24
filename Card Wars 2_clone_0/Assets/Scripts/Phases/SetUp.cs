using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUp : Phase
{
	[Server]
	public override void OnEnterPhase()
	{
		/// disable one of the players, based on gameManager host boolean
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

using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUp : Phase
{
	[ClientRpc]
	public override void OnEnterPhase()
	{
		Debug.Log("Entering Set Up Phase...");
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

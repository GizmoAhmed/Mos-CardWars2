using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : Phase
{
	[ClientRpc]
	public override void OnEnterPhase()
	{

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

using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseLand : Phase
{
	[ClientRpc]
	public override void OnEnterPhase()
	{
		Debug.Log("Phasing ChooseLand...");
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

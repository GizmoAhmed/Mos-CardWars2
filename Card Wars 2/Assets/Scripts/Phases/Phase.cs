using Mirror;
using Org.BouncyCastle.Bcpg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Phase : NetworkBehaviour
{
	protected GameManager gameManager;

	public virtual void Initialize(GameManager gameManager)
	{
		this.gameManager = gameManager;
	}

	[ClientRpc]
	public virtual void OnEnterPhase()
	{
	}

	[ClientRpc]
	public virtual void OnExitPhase()
	{
	}

	[Server]
	public virtual void HandlePhaseLogic() 
	{
	}
}

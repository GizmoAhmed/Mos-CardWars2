using Mirror;
using UnityEngine;

public class Attack : Phase
{
	public override void Initialize(GameManager gameManager)
	{
		base.Initialize(gameManager);
	}

	[ClientRpc]
	public override void OnEnterPhase()
	{
		Debug.Log("Attack scene would go down right here...");

		GameObject.Find("CombatManager").GetComponent<Combat>().InitializeCombat();

		// gameManager.currentPhase = GameManager.GamePhase.SetUp;
	}

	[ClientRpc]
	public override void OnExitPhase()
	{
		base.OnExitPhase();

		Debug.Log("Exiting attack...");
	}
}

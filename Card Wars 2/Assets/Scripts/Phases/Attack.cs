using Mirror;
using UnityEngine;

public class Attack : Phase
{
	public override void Initialize(GameManager gameManager)
	{
		base.Initialize(gameManager);
	}

	[Server]
	public override void OnEnterPhase()
	{
		Debug.Log("Enter Attack Phase:");

		FindObjectOfType<Combat>().InitializeCombat();

		Invoke("HandlePhaseLogic", 3f);
	}

	[Server]
	public override void HandlePhaseLogic()
	{
		gameManager.ChangePhase(GameManager.GamePhase.Attack, GameManager.GamePhase.SetUp);
	}

	[Server]
	public override void OnExitPhase()
	{
		base.OnExitPhase();

		Debug.Log("Exiting attack...");
	}
}

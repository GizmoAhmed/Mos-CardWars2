using Mirror;
using UnityEngine;

 // During this phase, players are disabled, to watch the attacks go down.

public class Attack : Phase
{
	public override void Initialize(GameManager gameManager)
	{
		base.Initialize(gameManager);
	}

	[ClientRpc]
	public override void OnEnterPhase()
	{
		Debug.Log("Entering Attack..");

		/*Player player = NetworkClient.localPlayer.GetComponent<Player>();
		player.myTurn = false;*/

		HandlePhaseLogic();
	}

	[ClientRpc]
	public override void OnExitPhase()
	{
		base.OnExitPhase();

		Debug.Log("Exiting attack...");

		/*Player player = NetworkClient.localPlayer.GetComponent<Player>();
		player.myTurn = true;*/
		
		// the set up phase, the phase immediately after this one, should disable one player
	}

	[Server]
	public override void HandlePhaseLogic()
	{
		Debug.Log("Attack scene would go down right here...");

		gameManager.currentPhase = GameManager.GamePhase.SetUp;
	}
}

using UnityEngine;
using Mirror;
using System.Collections;

public class Attack : Phase
{
	public override void Initialize(GameManager gameManager)
	{
		base.Initialize(gameManager);
	}

	/*[Server]
	public override void OnEnterPhase()
	{
		Player player0 = gameManager.Player0.identity.GetComponent<Player>();
		Player player1 = gameManager.Player1.identity.GetComponent<Player>();

		player0.RpcShowStats(player0.Health - player1.Score, "health");
		player1.RpcShowStats(player1.Health - player0.Score, "health");

		// reset the score at the start of each attack.
		player0.RpcShowStats(0, "score");
		player1.RpcShowStats(0, "score");

		DoBattle();
	}*/

	/*[Server]
	public void DoBattle()
	{
		StartCoroutine(DoBattleCoroutine());
	}*/

	/*private IEnumerator DoBattleCoroutine()
	{
		FindAnyObjectByType<Turns>().PlayerEnabler(null, "disableBoth");

		Player player0 = gameManager.Player0.identity.GetComponent<Player>();
		Player player1 = gameManager.Player1.identity.GetComponent<Player>();

		player0.searchComplete = player1.searchComplete = false;

		if (player0.myTurn)
		{
			player0.TargetStartBattleCardsSearch(player0.connectionToClient);

			yield return new WaitUntil(() => player0.searchComplete);

			player1.TargetStartBattleCardsSearch(player1.connectionToClient);

			yield return new WaitUntil(() => player1.searchComplete);
		}
		else
		{
			player1.TargetStartBattleCardsSearch(player1.connectionToClient);

			yield return new WaitUntil(() => player1.searchComplete);

			player0.TargetStartBattleCardsSearch(player0.connectionToClient);

			yield return new WaitUntil(() => player0.searchComplete);
		}

		HandlePhaseLogic();
	}*/

	[Server]
	public override void HandlePhaseLogic()
	{
		gameManager.ChangePhase(GameManager.GamePhase.Attack, GameManager.GamePhase.SetUp);
	}
}

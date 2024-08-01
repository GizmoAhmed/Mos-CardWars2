using UnityEngine;
using Mirror;
using System.Collections;

public class Attack : Phase
{
	public override void Initialize(GameManager gameManager)
	{
		base.Initialize(gameManager);
	}

	[Server]
	public override void OnEnterPhase()
	{
		DoBattle();
	}

	[Server]
	public void DoBattle()
	{
		StartCoroutine(DoBattleCoroutine());
	}

	private IEnumerator DoBattleCoroutine()
	{
		FindAnyObjectByType<Turns>().PlayerEnabler(null, "disableBoth");

		Player player0 = gameManager.Player0.identity.GetComponent<Player>();
		Player player1 = gameManager.Player1.identity.GetComponent<Player>();

		player0.searchComplete = player0.searchComplete = false;

		if (player0.myTurn)
		{
			Debug.Log("player 0...");
			player0.TargetStartBattleCardsSearch(player0.connectionToClient);

			yield return new WaitUntil(() => player0.searchComplete);

			Debug.Log("...then player 1");
			player1.TargetStartBattleCardsSearch(player1.connectionToClient);

			yield return new WaitUntil(() => player1.searchComplete);
		}
		else
		{
			Debug.Log("player 1...");
			player1.TargetStartBattleCardsSearch(player1.connectionToClient);

			yield return new WaitUntil(() => player1.searchComplete);

			Debug.Log("...then player 0");
			player0.TargetStartBattleCardsSearch(player0.connectionToClient);

			yield return new WaitUntil(() => player0.searchComplete);
		}

		HandlePhaseLogic();
	}

	[Server]
	public override void HandlePhaseLogic()
	{
		Debug.Log("Handling Attack Phase Logic");
		gameManager.ChangePhase(GameManager.GamePhase.Attack, GameManager.GamePhase.SetUp);
	}
}

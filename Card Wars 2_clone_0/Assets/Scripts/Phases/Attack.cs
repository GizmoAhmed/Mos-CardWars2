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

		if (player0.myTurn)
		{
			Debug.Log("player 0...");
			yield return StartCoroutine(player0.RpcFindBattleCardsCoroutine());
			Debug.Log("...then player 1");
			yield return StartCoroutine(player1.RpcFindBattleCardsCoroutine()); // not running for some reason

		}
		else
		{
			Debug.Log("player 1...");
			yield return StartCoroutine(player1.RpcFindBattleCardsCoroutine());
			Debug.Log("...then player 0");
			yield return StartCoroutine(player0.RpcFindBattleCardsCoroutine());
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
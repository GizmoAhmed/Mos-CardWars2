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
		DoBattle();
	
		Invoke("HandlePhaseLogic", 3f);
	}

	[Server]
	public override void HandlePhaseLogic()
	{
		gameManager.ChangePhase(GameManager.GamePhase.Attack, GameManager.GamePhase.SetUp);
	}

	[Server]
	public void DoBattle()
	{
		FindAnyObjectByType<Turns>().ManageTurn(null, "disableBoth");

		Player player0 = gameManager.Player0.identity.GetComponent<Player>();
		Player player1 = gameManager.Player1.identity.GetComponent<Player>();

		if (player0.myTurn)
		{
			Debug.LogWarning(">>> player0 is attacking first <<<");
			player0.RpcFindBattleCards();

			Invoke("OneBattle", 2f);
		}
		else
		{
			Debug.LogWarning(">>> player0 is attacking first <<<");
			player1.RpcFindBattleCards();

			Invoke("ZeroBattle", 2f);
		}
	}

	public void ZeroBattle() 
	{
		Debug.LogWarning("<<< player0 is attacking second >>>");

		Player p0 = gameManager.Player0.identity.GetComponent<Player>();

		p0.RpcFindBattleCards();
	}

	public void OneBattle()
	{
		Debug.LogWarning("<<< player1 is attacking second >>>");

		Player p1 = gameManager.Player1.identity.GetComponent<Player>();

		p1.RpcFindBattleCards();
	}
}

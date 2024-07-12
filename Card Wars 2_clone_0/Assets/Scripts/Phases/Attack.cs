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
	public override void OnExitPhase()
	{

	}

	// at the start of every attack.cs...
	[Server]
	public void DoBattle()
	{
		FindAnyObjectByType<Turns>().ManageTurn(null, "disableBoth");

		Player player0 = gameManager.Player0.identity.GetComponent<Player>();
		Player player1 = gameManager.Player1.identity.GetComponent<Player>();

		if (player0.myTurn)
		{
			Debug.Log($"Player {gameManager.Player0.connectionId} is attacking first");
			player0.FindBattleCards();
			Debug.Log($"...Then Player {gameManager.Player1.connectionId} attacks after");
			//DoBattle(FindAnyObjectByType<GameManager>().Player1); 
		}
		else
		{
			Debug.Log($"Player {gameManager.Player1.connectionId} is attacking first...");
			player1.FindBattleCards();
			Debug.Log($"...Then Player {gameManager.Player0.connectionId} attacks after");
			// DoBattle(FindAnyObjectByType<GameManager>().Player0); 
		}
	}
}

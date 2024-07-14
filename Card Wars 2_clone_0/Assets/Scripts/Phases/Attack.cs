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

	// at the start of every attack.cs...
	[Server]
	public void DoBattle()
	{
		FindAnyObjectByType<Turns>().ManageTurn(null, "disableBoth");

		Player player0 = gameManager.Player0.identity.GetComponent<Player>();
		Player player1 = gameManager.Player1.identity.GetComponent<Player>();

		if (player0.myTurn)
		{
			Debug.Log("Server: Player " + player0.connectionToClient.connectionId.ToString() + " goes first");
			player0.RpcFindBattleCards();

			Debug.Log("Server: Player" + player1.connectionToClient.connectionId.ToString() + " goes second, but not really for now");
			// player1.RpcFindBattleCards();
		}
		else
		{
			Debug.Log("Server: Player " + player1.connectionToClient.connectionId.ToString() + " goes first");
			player1.RpcFindBattleCards();

			Debug.Log("Server: Player " + player0.connectionToClient.connectionId.ToString() + " goes second, but not really for now");
			// player0.RpcFindBattleCards();
		}
	}
}

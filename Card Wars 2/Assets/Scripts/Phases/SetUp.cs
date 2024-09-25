using Mirror;
using UnityEngine;

public class SetUp : Phase
{
	[SyncVar] public int MoneyAddOn;
	[SyncVar] public bool firstSetUp;

	[Server]
	public override void Initialize(GameManager gameManager)
	{
		base.Initialize(gameManager);
		MoneyAddOn = 2;
		firstSetUp = true;
	}

	[Server]
	public override void OnEnterPhase()
	{
		Player player0 = gameManager.Player0.identity.GetComponent<Player>();
		Player player1 = gameManager.Player1.identity.GetComponent<Player>();

		if (player0.Health == 0)
		{
			Debug.Log("player1 wins");
		}
		else if (player0.Health == 1) 
		{
			Debug.Log("player0 wins");
		}

		if (firstSetUp)
		{
			player0.RpcShowStats(gameManager.firstMagic, "max_magic");
			player1.RpcShowStats(gameManager.firstMagic, "max_magic");
			
			player0.RpcShowStats(gameManager.firstMagic, "current_magic");
			player1.RpcShowStats(gameManager.firstMagic, "current_magic");

			player0.RpcShowStats(gameManager.firstMoney, "money");
			player1.RpcShowStats(gameManager.firstMoney, "money");

			firstSetUp = false;
		}
		else
		{
			player0.RpcShowStats(player0.MaxMagic, "max_magic");
			player1.RpcShowStats(player1.MaxMagic, "max_magic");

			player0.RpcShowStats(player0.MaxMagic, "current_magic");
			player1.RpcShowStats(player1.MaxMagic, "current_magic");

			player0.RpcShowStats(player0.Money + MoneyAddOn, "money");
			player1.RpcShowStats(player1.Money + MoneyAddOn, "money");
		}

		gameManager.turnManager.PlayerEnabler(null);
	}

	[Server]
	public override void OnExitPhase()
	{
		gameManager.turnManager.IncrementTurn();
	}
}

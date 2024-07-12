using Mirror;
using UnityEngine;

public class SetUp : Phase
{
	[Header("SET IN INSPECTOR")]
	public int NextMagic;
	public int MoneyAddOn;

	[SyncVar] public bool firstSetUp;

	[Server]
	public override void Initialize(GameManager gameManager)
	{
		base.Initialize(gameManager);
		firstSetUp = true;
		NextMagic = 0;
		MoneyAddOn = 2;
	}

	[Server]
	public override void OnEnterPhase()
	{
		Player player0 = gameManager.Player0.identity.GetComponent<Player>();
		Player player1 = gameManager.Player1.identity.GetComponent<Player>();

		if (firstSetUp == true)
		{
			player0.RpcShowConsumable(gameManager.firstMagic, "magic");
			player1.RpcShowConsumable(gameManager.firstMagic, "magic");

			player0.RpcShowConsumable(gameManager.firstMoney, "money");
			player1.RpcShowConsumable(gameManager.firstMoney, "money");

			firstSetUp = false;
		}
		else 
		{
			player0.RpcShowConsumable(gameManager.firstMagic + NextMagic, "magic");
			player1.RpcShowConsumable(gameManager.firstMagic + NextMagic, "magic");

			player0.RpcShowConsumable(player0.Money + MoneyAddOn, "money");
			player1.RpcShowConsumable(player1.Money + MoneyAddOn, "money");
		}

		NextMagic++;

		gameManager.turnManager.ManageTurn(null);
	}

	[Server]
	public override void OnExitPhase()
	{
		gameManager.turnManager.IncrementTurn();
	}
}

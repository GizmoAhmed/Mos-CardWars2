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

		if (firstSetUp)
		{
			player0.RpcShowStats(gameManager.firstMagic, "max_magic");
			player1.RpcShowStats(gameManager.firstMagic, "max_magic");
			
			player0.RpcShowStats(gameManager.firstMagic, "current_magic");
			player1.RpcShowStats(gameManager.firstMagic, "current_magic");

			player0.RpcShowStats(gameManager.firstMoney, "money");
			player1.RpcShowStats(gameManager.firstMoney, "money");

			/*player0.CmdShowStats(player0.deck.MyDeck.Count, "cards");
			player0.CmdShowStats(player0.deck.MyDeck.Count, "decksize");

			player1.CmdShowStats(player1.deck.MyDeck.Count, "cards");
			player1.CmdShowStats(player1.deck.MyDeck.Count, "decksize");*/

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

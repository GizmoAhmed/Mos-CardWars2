using Mirror;
using UnityEngine;

public class SetUp : Phase
{
	[Header("SET IN INSPECTOR")]
	public int MagicAddOn;
	public int NextMoney;

	[SyncVar] public bool firstSetUp;

	[Server]
	public override void Initialize(GameManager gameManager)
	{
		base.Initialize(gameManager);
		firstSetUp = true;
	}

	[Server]
	public override void OnEnterPhase()
	{
		Debug.Log("Entering set up phase");

		Player player0 = gameManager.Player0.identity.GetComponent<Player>();
		Player player1 = gameManager.Player1.identity.GetComponent<Player>();

		if (firstSetUp == true)
		{
			player0.Magic = gameManager.firstMagic;
			player1.Magic = gameManager.firstMagic;

			player0.Money = gameManager.firstMoney;
			player1.Money = gameManager.firstMoney;
		}
		else 
		{
			player0.Magic = gameManager.firstMagic + MagicAddOn;
			player1.Magic = gameManager.firstMagic + MagicAddOn;

			player0.Money += NextMoney;
			player1.Money += NextMoney;
		}

		MagicAddOn++;

		gameManager.turnManager.ManageTurn(null);
	}

	[Server]
	public override void OnExitPhase()
	{
		gameManager.IncrementTurn();
	}
}

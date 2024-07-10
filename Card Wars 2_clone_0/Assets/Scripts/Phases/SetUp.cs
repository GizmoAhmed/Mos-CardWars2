using Mirror;
using UnityEngine;

public class SetUp : Phase
{
	[SyncVar] public int MagicAddOn;
	

	[Server]
	public override void Initialize(GameManager gameManager)
	{
		this.gameManager = gameManager;
		MagicAddOn = 0;
	}

	[Server]
	public override void OnEnterPhase()
	{
		Debug.Log("Entering set up phase");

		/*
		 * Since player knows its own magic, You should make a clientRpc in player for 
		 * setting consumables for every set up phase after the first
		 * 
		 * For each player, set consumables player.RpcSetConsumables
		 */

		gameManager.SetConsumables(gameManager.startingMagic + MagicAddOn, gameManager.startingMoney);

		MagicAddOn++;

		gameManager.turnManager.ManageTurn(null);
	}

	[Server]
	public override void OnExitPhase()
	{
		gameManager.IncrementTurn();
	}
}

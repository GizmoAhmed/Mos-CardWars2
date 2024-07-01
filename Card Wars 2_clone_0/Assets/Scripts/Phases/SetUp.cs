using Mirror;
using UnityEngine;

public class SetUp : Phase
{
	[SyncVar] public int MagicAddOn;

	public NetworkConnectionToClient Player0;
	public NetworkConnectionToClient Player1;

	[Server]
	public override void Initialize(GameManager gameManager)
	{
		this.gameManager = gameManager;
		MagicAddOn = 0;
	}

	[Server]
	public void IdentitfyPlayers() 
	{
		foreach (var conn in NetworkServer.connections.Values)
		{
			if (Player0 is null)
				{ Player0 = conn; }
			else
				{ Player1 = conn; }
		}
	}

	[Server]
	public override void OnEnterPhase()
	{
		// Disable one of the players, based on gameManager host boolean
		Debug.Log("Entering a set up phase");

		// Find a way to update the money, which +2 of whatever you had last turn
		gameManager.SetConsumables(gameManager.startingMagic + MagicAddOn, gameManager.startingMoney);

		MagicAddOn++;

		HandlePhaseLogic();
	}

	[Server]
	public override void OnExitPhase()
	{

	}

	[Server]
	public override void HandlePhaseLogic()
	{
		// remember to change turns
		Player player0 = Player0.identity.GetComponent<Player>();
		Player player1 = Player1.identity.GetComponent<Player>();

		if (gameManager.hostFirst)
		{
			Debug.Log($"Player {Player0.connectionId} will set up first");

		}
		else 
		{
			Debug.Log($"Player {Player1.connectionId} will set up first");
		}
	}
}

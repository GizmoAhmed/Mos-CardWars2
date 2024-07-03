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
		Debug.Log("Entering a set up phase");

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
		Player player0 = Player0.identity.GetComponent<Player>();
		Player player1 = Player1.identity.GetComponent<Player>();

		player0.RpcTurnMessage(gameManager.hostFirst);
		player1.RpcTurnMessage(!gameManager.hostFirst);
	}
}

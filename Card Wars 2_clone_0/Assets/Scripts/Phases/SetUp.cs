using Mirror;
using UnityEngine;

public class SetUp : Phase
{
	[SyncVar] public int MagicAddOn;

	[SyncVar] public bool alternate;

	private NetworkConnectionToClient Player0;
	private NetworkConnectionToClient Player1;

	private Player player0;
	private Player player1;

	[Server]
	public override void Initialize(GameManager gameManager)
	{
		this.gameManager = gameManager;
		MagicAddOn = 0;
		alternate = true;
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

		player0 = Player0.identity.GetComponent<Player>();
		player1 = Player1.identity.GetComponent<Player>();
	}

	[Server]
	public override void OnEnterPhase()
	{
		Debug.Log("Entering a set up phase");

		/*
		 * since player knows its magic
		 * client rpc in player for setting consumabels for every set up phase after the first
		 * for each player, set consumables player.RpcSetConsumables
		 */

		gameManager.SetConsumables(gameManager.startingMagic + MagicAddOn, gameManager.startingMoney);

		MagicAddOn++;

		HandlePhaseLogic();
	}

	[Server]
	public override void OnExitPhase()
	{
		gameManager.IncrementTurn();
	}

	[Server]
	public override void HandlePhaseLogic()
	{
		ManageTurn(null);
	}

	[Server]
	public void ManageTurn(NetworkConnectionToClient conn)
	{
		// default, the first set up dictated by hostFirst.
		if (conn == null)
		{
			player0.RpcTurnMessage(gameManager.hostFirst);
			player0.RpcEnablePlayer(gameManager.hostFirst);

			player1.RpcTurnMessage(!gameManager.hostFirst);
			player1.RpcEnablePlayer(!gameManager.hostFirst);
		}
		else 
		{
			Player thisPlayer = conn.identity.GetComponent<Player>();

			if (thisPlayer == player0)
			{
				player0.RpcTurnMessage(false);
				player0.RpcEnablePlayer(false);

				player1.RpcTurnMessage(true);
				player1.RpcEnablePlayer(true);
			}
			else if (thisPlayer == player1)
			{
				player0.RpcTurnMessage(true);
				player0.RpcEnablePlayer(true);

				player1.RpcTurnMessage(false);
				player1.RpcEnablePlayer(false);
			}
			else 
			{
				Debug.LogError("! The Player passed isn't familiar to Set up !");
			}
		}

		
	}
}

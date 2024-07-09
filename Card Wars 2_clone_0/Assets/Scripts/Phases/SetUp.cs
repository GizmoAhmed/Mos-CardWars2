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
		alternate = gameManager.hostFirst;
		Debug.Log("Init: alternate = hostFirst = " + alternate.ToString());
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

		// so combat.cs can use these conns later
		GameObject.Find("CombatManager").GetComponent<Combat>().FindPlayers(Player0, Player1);
	}

	[Server]
	public override void OnEnterPhase()
	{
		Debug.Log("Entering set up phase");

		/*
		 * since player knows its magic
		 * client rpc in player for setting consumabels for every set up phase after the first
		 * for each player, set consumables player.RpcSetConsumables
		 */

		gameManager.SetConsumables(gameManager.startingMagic + MagicAddOn, gameManager.startingMoney);

		MagicAddOn++;

		ManageTurn(null);
	}

	[Server]
	public void ManageTurn(NetworkConnectionToClient conn, string mode = "default")
	{
		switch (mode)
		{
			case "disableBoth":
				SetPlayerState(player0, false);
				SetPlayerState(player1, false);
				break;
			/*case "player0":
				SetPlayerState(player0, true);
				SetPlayerState(player1, false);
				break;
			case "player1":
				SetPlayerState(player0, false);
				SetPlayerState(player1, true);
				break;*/
			default:
				if (conn == null)
				{
					// start of set-up uses alternate...
					Debug.Log("alternate changing from " + alternate.ToString());

					SetPlayerState(player0, alternate);
					SetPlayerState(player1, !alternate);
					
					// and then alternate flips so that its the opposite the next time setup starts 
					alternate = !alternate;

					Debug.Log(" to " + (alternate).ToString());
				}
				else
				{
					Player thisPlayer = conn.identity.GetComponent<Player>();

					if (thisPlayer == player0)
					{
						SetPlayerState(player0, false);
						SetPlayerState(player1, true);
					}
					else if (thisPlayer == player1)
					{
						SetPlayerState(player0, true);
						SetPlayerState(player1, false);
					}
					else
					{
						Debug.LogError("! The Player passed isn't familiar to Setup !");
					}
				}
				break;
		}
	}

	private void SetPlayerState(Player player, bool state)
	{
		player.RpcTurnMessage(state);
		player.RpcEnablePlayer(state);
	}

	[Server]
	public override void OnExitPhase()
	{
		gameManager.IncrementTurn();
	}
}

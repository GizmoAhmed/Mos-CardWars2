using Mirror;
using UnityEngine;

public class Turns : NetworkBehaviour
{
	[SyncVar] public bool	alternate;
	[SyncVar] public int	TurnCount;
	protected GameManager	gameManager;

	[Server]
	public void InitializeTurns(GameManager gameManager)
	{
		this.gameManager = gameManager;
		TurnCount = 1;
		alternate = gameManager.HostGoesFirst;
	}

	[Server]
	public void IncrementTurn()
	{
		TurnCount++;
		// update all spells and building timers here mabye
		foreach (var conn in NetworkServer.connections.Values)
		{
			conn.identity.GetComponent<Player>().RpcUpdateTurnText(TurnCount);
		}
	}

	[Server]
	public void ManageTurn(NetworkConnectionToClient conn, string mode = "default")
	{
		Player player0 = gameManager.Player0.identity.GetComponent<Player>();
		Player player1 = gameManager.Player1.identity.GetComponent<Player>();

		switch (mode)
		{
			case "disableBoth":

				// Debug.Log("Disabling Both Players");

				SetPlayerState(player0, false);
				SetPlayerState(player1, false);
				break;

			default:
				if (conn == null)
				{
					SetPlayerState(player0, alternate);
					SetPlayerState(player1, !alternate);

					// alternate flips so that its the opposite the next time setup starts 
					alternate = !alternate;
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
}

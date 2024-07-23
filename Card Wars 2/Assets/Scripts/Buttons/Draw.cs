using UnityEngine;
using Mirror;

public class Draw : NetworkBehaviour
{
	private Player player;
	
	public void DrawCard()
	{
		NetworkIdentity networkIdentity = NetworkClient.connection.identity;
		player = networkIdentity.GetComponent<Player>();

		if (player.DrawCost <= player.Money)
		{
			player.CmdShowStats(player.Money - player.DrawCost, "money");
			player.deck.CmdDrawCard();
		}
	}
}

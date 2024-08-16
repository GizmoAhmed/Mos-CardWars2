using UnityEngine;
using Mirror;

public class Draw : NetworkBehaviour
{
	private Player player;
	
	public void DrawCard()
	{
		NetworkIdentity networkIdentity = NetworkClient.connection.identity;
		player = networkIdentity.GetComponent<Player>();

		if (player.DrawCost <= player.Money && player.deck.MyDeck.Count > 0) 
		{
			player.CmdChangeStats(player.Money - player.DrawCost, "money");
			player.deck.CmdDrawCard();
		}
		else 
		{
			Debug.LogWarning("not enough money or enough cards");
		}
	}
}

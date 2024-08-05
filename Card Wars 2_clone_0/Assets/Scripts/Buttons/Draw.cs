using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Draw : NetworkBehaviour
{
	private Player player;
	
	public void DrawCard()
	{
		NetworkIdentity networkIdentity = NetworkClient.connection.identity;
		player = networkIdentity.GetComponent<Player>();

		if (player.DrawCost <= player.Money && player.deck.MyDeck.Count > 0) 
		{
			player.CmdShowStats(player.Money - player.DrawCost, "money");
			
			int randomIndex = Random.Range(0, player.deck.MyDeck.Count);
			GameObject cardInstance = player.deck.MyDeck[randomIndex];

			player.deck.CmdDrawCard(cardInstance);

			player.deck.MyDeck.RemoveAt(randomIndex);

		}
		else 
		{
			Debug.LogWarning("not enough money or enough cards");
		}
	}
}

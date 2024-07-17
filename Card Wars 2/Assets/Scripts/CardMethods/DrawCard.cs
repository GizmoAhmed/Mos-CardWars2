using UnityEngine;
using Mirror;

public class DrawCard : NetworkBehaviour
{
	private Player player;
	
	public void Draw()
	{
		NetworkIdentity networkIdentity = NetworkClient.connection.identity;
		player = networkIdentity.GetComponent<Player>();

		if (player.DrawCost > player.Money)
		{
			Debug.Log("To Expensive...");
		}
		else 
		{
			player.CmdShowConsumable(player.Money - player.DrawCost,"money");
			player.deck.CmdDrawCard();
		}
	}
}

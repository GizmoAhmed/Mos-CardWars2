using UnityEngine;
using Mirror;

public class DrawCard : NetworkBehaviour
{
	private Player player;
	
	public void Draw()
	{
		NetworkIdentity networkIdentity = NetworkClient.connection.identity;
		player = networkIdentity.GetComponent<Player>();

		if (player.Cost > player.Money)
		{
			Debug.Log("To Expensive...");
		}
		else 
		{
			player.Money -= player.Cost;

			player.CmdDrawCard();
		}
	}
}

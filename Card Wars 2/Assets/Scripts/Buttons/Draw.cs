using UnityEngine;
using Mirror;

public class Draw : NetworkBehaviour
{
	private Player player;
	
	public void DrawCards()
	{
		NetworkIdentity networkIdentity = NetworkClient.connection.identity;
		player = networkIdentity.GetComponent<Player>();

		player.deck.CmdDrawCard();
		player.deck.CmdDrawCard();
		player.deck.CmdDrawCard();
	}
}

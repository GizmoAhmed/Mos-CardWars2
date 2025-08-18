using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class Deck : NetworkBehaviour
{
	// currently only server player gets to see updated lists
	 public List<GameObject> myDeck;
	 
	[Command] public void CmdDrawCard() { DrawCardFromDeck(connectionToClient); }

	private void DrawCardFromDeck(NetworkConnectionToClient conn)
	{
		if (myDeck.Count == 0)
		{
			Debug.LogWarning($"Empty Deck, Player {connectionToClient.connectionId + 1} Can't Draw");
			return;
		}

		Player player = GetComponentInParent<Player>();

		int randomIndex = Random.Range(0, myDeck.Count);
		GameObject cardInstance = myDeck[randomIndex];

		GameObject drawnCard = Instantiate(cardInstance);
		NetworkServer.Spawn(drawnCard, conn);

		player.cardHandler.RpcHandleCard(drawnCard,null);

		myDeck.RemoveAt(randomIndex);
	}
}

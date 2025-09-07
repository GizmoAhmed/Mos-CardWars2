using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace PlayerStuff
{
	public class DeckCollection : NetworkBehaviour
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

			if (player == null)
			{
				Debug.LogError("Player is null, can't draw");
				return;
			}

			if (player.playerStats.money >= player.playerStats.drawCost)
			{
				int randomIndex = Random.Range(0, myDeck.Count);
				GameObject cardInstance = myDeck[randomIndex];
    
				// add to scene
				GameObject drawnCard = Instantiate(cardInstance);
            
				// set owner to player who drew it
				CardStats cardStats = drawnCard.GetComponent<CardStats>();
				cardStats.thisCardOwner = player.playerStats; 

				// add it to the server for both players
				NetworkServer.Spawn(drawnCard, conn);

				player.cardHandler.RpcHandleCard(drawnCard, null);

    
				myDeck.RemoveAt(randomIndex);
            
				player.playerStats.money -= player.playerStats.drawCost;
			}
			else
			{
				Debug.LogWarning($"Player {connectionToClient.connectionId + 1} doesn't have enough money to Draw");
			}
		}
	}
}

using Mirror;
using System.Collections.Generic;
using UnityEngine;
using static Card;

public class Deck : NetworkBehaviour
{
	// for different decks, you may need to add cards from a data base in start.
	public List<GameObject> MyDeck;

	public override void OnStartClient() 
	{
		GameManager game = FindAnyObjectByType<GameManager>();

		List<GameObject> deck;

		if (game.chooseDeck == 1)
		{
			deck = game.MasterDeck;
		}
		else if (game.chooseDeck == 2)
		{
			deck = game.allCreaturesDeck;
		}
		else
		{
			deck = game.debugDeck;
		} 

		foreach (GameObject cardOB in deck) { MyDeck.Add(cardOB); }
	}

	[Command] public void CmdDrawCard() { DrawCardFromDeck(connectionToClient); }

	private void DrawCardFromDeck(NetworkConnectionToClient conn)
	{
		Player player = GetComponentInParent<Player>();

		int randomIndex = Random.Range(0, MyDeck.Count);
		GameObject cardInstance = MyDeck[randomIndex];

		GameObject drawnCard = Instantiate(cardInstance);
		NetworkServer.Spawn(drawnCard, conn);

		player.RpcHandleCard(drawnCard, CardState.Hand, null);

		MyDeck.RemoveAt(randomIndex);
	}
}

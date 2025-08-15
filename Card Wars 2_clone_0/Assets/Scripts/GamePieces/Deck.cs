using Mirror;
using System.Collections.Generic;
using UnityEngine;
using static Card;

public class Deck : NetworkBehaviour
{
	// currently only server player gets to see updated lists
	 public List<GameObject> MyDeck;

	private void Start()
	{
		/*GameManager game = FindAnyObjectByType<GameManager>();

		List<GameObject> deck;

		if (game.deckType == GameManager.DeckType.Player)
		{
			if (game.p0RecievedDeck == false)
			{
				deck = game.p0Deck;
				game.p0RecievedDeck = true;
			}
			else 
			{
				deck = game.p1Deck;
			}
		}
		else 
		{
			if (game.chooseDeck == 1)
			{
				deck = game.DebugMaster;
			}
			else if (game.chooseDeck == 2)
			{
				deck = game.DebugAllCreatures;
			}
			else
			{
				deck = game.DebugMisc;
			}
		}

		foreach (GameObject cardOB in deck) { MyDeck.Add(cardOB); }*/
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

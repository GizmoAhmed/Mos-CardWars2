using Mirror;
using System.Collections.Generic;
using UnityEngine;
using static Card;

public class Deck : NetworkBehaviour
{
	public List<GameObject> MyDeck;
	public int MaxSize;

	public void InitDeck() 
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

		MaxSize = MyDeck.Count;
	}

	[Command] public void CmdDrawCard()
	{
		DrawCardFromDeck(connectionToClient);
	}

	public void DrawCardFromDeck(NetworkConnectionToClient conn)
	{
		Player player = conn.identity.GetComponent<Player>();

		int randomIndex = Random.Range(0, MyDeck.Count);
		GameObject cardInstance = MyDeck[randomIndex];

		GameObject drawnCard = Instantiate(cardInstance);
		NetworkServer.Spawn(drawnCard, conn);

		player.RpcUpdateDeck(randomIndex);

		player.RpcHandleCard(drawnCard, CardState.Hand, null);
	}
}

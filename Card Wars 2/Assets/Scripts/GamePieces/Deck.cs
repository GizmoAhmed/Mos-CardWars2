using Mirror;
using System.Collections.Generic;
using UnityEngine;
using static Card;

public class Deck : NetworkBehaviour
{
	private	GameManager	game;
	private	Player		player;

	public	List<GameObject> MyDeck;

	public void InitializeDeck(int ID) 
	{
		game = FindAnyObjectByType<GameManager>();

		player = GetComponentInParent<Player>();

		List<GameObject> deck = (ID == 0) ? game.p0Deck : game.p1Deck;

		foreach (GameObject cardOB in deck) { MyDeck.Add(cardOB); }
	}

	[Command] public void CmdDrawCard(GameObject cardInstance) { DrawCardFromDeck(cardInstance, connectionToClient); }

	private void DrawCardFromDeck(GameObject cardInstance, NetworkConnectionToClient conn)
	{ 
		GameObject drawnCard = Instantiate(cardInstance);
		NetworkServer.Spawn(drawnCard, conn);

		player.RpcHandleCard(drawnCard, CardState.Hand, null);
	}
}

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

		deck = (game.chooseDeck == 1) ? game.MasterDeck : game.debugDeck; 

		foreach (GameObject cardOB in deck) { MyDeck.Add(cardOB); }
	}

	[Command] public void CmdDrawCard() { DrawCardFromDeck(connectionToClient); }

	private void DrawCardFromDeck(NetworkConnectionToClient conn)
	{
		Player player = GetComponentInParent<Player>();

		if (MyDeck.Count > 0)
		{
			int randomIndex = Random.Range(0, MyDeck.Count);
			GameObject cardInstance = MyDeck[randomIndex];

			GameObject drawnCard = Instantiate(cardInstance);
			NetworkServer.Spawn(drawnCard, conn);

			Card cardScript = drawnCard.GetComponent<Card>();
			cardScript.SetState(CardState.Hand);

			player.RpcShowCard(drawnCard, CardState.Hand, null);

			MyDeck.RemoveAt(randomIndex);
		}
		else 
		{
			Debug.LogWarning("Empty Deck, Can't Draw");
		}
	}
}

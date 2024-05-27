using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static Card;

public class PlayerManager : NetworkBehaviour
{
	public GameObject Hand;
	public GameObject Player1Area;
	public GameObject Player2Area;
	public GameObject Deck;

	private List<GameObject> pCardsInDeck; // Local reference to the deck's cards

	public override void OnStartClient()
	{
		base.OnStartClient();

		Hand = GameObject.Find("Hand");
		Player1Area = GameObject.Find("Player1Area");
		Player2Area = GameObject.Find("Player2Area");
		Deck = GameObject.Find("Deck");
	}

/*	private void Awake()
	{
		Debug.Log("Spawning...");

		Deck deckComponent = Deck.GetComponent<Deck>();

		pCardsInDeck = deckComponent.CardsInDeck;

		for (int i = 0; i < pCardsInDeck.Count; i++)
		{
			NetworkServer.Spawn(pCardsInDeck[i], connectionToClient);
			GameObject card = Instantiate(pCardsInDeck[i], new Vector2(0, 0), Quaternion.identity);
			NetworkServer.Spawn(card, connectionToClient);
		}
	}*/

	[Server]
	public override void OnStartServer() 
	{
		base.OnStartServer();
	}

	[Command]
	public void CmdDrawCard(List<GameObject> CardsInDeck)
	{
		if (CardsInDeck.Count > 0)
		{
			int randomIndex = Random.Range(0, CardsInDeck.Count);

			GameObject drawnCard = Instantiate(CardsInDeck[randomIndex], new Vector2(0, 0), Quaternion.identity);

			NetworkServer.Spawn(drawnCard, connectionToClient);

			drawnCard.transform.SetParent(Hand.transform, false);

			CardsInDeck.RemoveAt(randomIndex);

			Card cardScript = drawnCard.GetComponent<Card>();

			if (cardScript != null)
			{
				cardScript.SetState(CardState.Hand);
			}
		}

	}
}

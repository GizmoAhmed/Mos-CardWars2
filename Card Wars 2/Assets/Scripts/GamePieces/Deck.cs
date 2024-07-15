using Mirror;
using System.Collections.Generic;
using UnityEngine;
using static Card;

public class Deck : NetworkBehaviour
{
	// for different decks, you may need to add cards from a data base in start.
	public List<GameObject> MyDeck;

	[SyncVar] public int DeckSize;

	public Player player;

	private void Start()
	{
		player = GetComponentInParent<Player>();
	}

	
	public override void OnStartClient() 
	{
		GameManager game = FindAnyObjectByType<GameManager>();

		foreach (GameObject cardOB in game.MasterDeck) 
		{
			MyDeck.Add(cardOB);
		}

		DeckSize = MyDeck.Count;
	}

	[Command] 
	public void CmdDrawCard()
	{
		DrawCardFromDeck(MyDeck, connectionToClient);
	}

	// [Server] maybe add this
	private void DrawCardFromDeck(List<GameObject> cardList, NetworkConnectionToClient conn)
	{
		if (cardList.Count > 0)
		{
			int randomIndex = Random.Range(0, cardList.Count);
			GameObject cardInstance = cardList[randomIndex];

			GameObject drawnCard = Instantiate(cardInstance);
			NetworkServer.Spawn(drawnCard, conn);

			Card cardScript = drawnCard.GetComponent<Card>();
			cardScript.SetState(CardState.Hand);

			player.RpcShowCard(drawnCard, CardState.Hand, null);

			cardList.RemoveAt(randomIndex);
		}
		else 
		{
			Debug.Log("Empty Deck, Can't Draw");
		}

		DeckSize = MyDeck.Count;
	}
}

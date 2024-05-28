using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static Card;

public class PlayerManager : NetworkBehaviour
{
	public GameObject Hand;
	public GameObject Player1Area;
	public GameObject Player2Area;

	public List<GameObject> CardsInDeck;

	public override void OnStartClient()
	{
		base.OnStartClient();
		Hand = GameObject.Find("Hand");
		Player1Area = GameObject.Find("Player1Area");
		Player2Area = GameObject.Find("Player2Area");
	}

	[Command]
	public void CmdDrawCard()
	{
		if (CardsInDeck.Count > 0)
		{
			int randomIndex = Random.Range(0, CardsInDeck.Count);
			GameObject cardPrefab = CardsInDeck[randomIndex];

			GameObject drawnCard = Instantiate(cardPrefab);
			NetworkServer.Spawn(drawnCard, connectionToClient);

			RpcHandleDrawnCard(drawnCard);

			CardsInDeck.RemoveAt(randomIndex);
		}
	}

	[ClientRpc]
	void RpcHandleDrawnCard(GameObject drawnCard)
	{
		drawnCard.transform.SetParent(Hand.transform, false);

		Card cardScript = drawnCard.GetComponent<Card>();
		if (cardScript != null)
		{
			cardScript.SetState(CardState.Hand);
		}
	}
}

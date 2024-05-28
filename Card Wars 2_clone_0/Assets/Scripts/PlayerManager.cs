using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static Card;

public class PlayerManager : NetworkBehaviour
{
	public GameObject Hand1;
	public GameObject Hand2;

	public GameObject Player1Area;
	public GameObject Player2Area;

	public List<GameObject> Cards1;
	public List<GameObject> Cards2;

	public override void OnStartClient()
	{
		base.OnStartClient();

		Hand1 = GameObject.Find("Hand1");
		Hand2 = GameObject.Find("Hand2");

		Player1Area = GameObject.Find("Player1Area");
		Player2Area = GameObject.Find("Player2Area");
	}

	[Server]
	public override void OnStartServer()
	{
		base.OnStartServer();
	}

	[Command] // client asks server to do something
	public void CmdDrawCard()
	{
		if (isOwned)
		{
			DrawCardFromDeck(Cards1, connectionToClient);
		}
		else
		{
			DrawCardFromDeck(Cards2, connectionToClient);
		}
	}

	private void DrawCardFromDeck(List<GameObject> cardList, NetworkConnectionToClient conn)
	{
		if (cardList.Count > 0)
		{
			int randomIndex = Random.Range(0, cardList.Count);
			GameObject cardPrefab = cardList[randomIndex];

			GameObject drawnCard = Instantiate(cardPrefab);
			NetworkServer.Spawn(drawnCard, conn);

			Card cardScript = drawnCard.GetComponent<Card>();
			cardScript.SetState(CardState.Drawn);

			RpcShowCard(drawnCard, CardState.Drawn);

			cardList.RemoveAt(randomIndex);
		}
	}

	[ClientRpc] // server asks client(s) to do something
	void RpcShowCard(GameObject card, CardState state)
	{
		if (state == CardState.Drawn)
		{
			if (isOwned)
			{
				card.transform.SetParent(Hand1.transform, false);
			}
			else
			{
				card.transform.SetParent(Hand2.transform, false);
			}
		}
	}
}

using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static Card;
using UnityEngine.UIElements;

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
		// Initialize server-specific data here if needed
	}

	[Command] // Client asks server to do something
	public void CmdDrawCard()
	{
		// Ensure that the command is being executed on the server
		if (!isServer)
		{
			Debug.LogError("CmdDrawCard called on a client that is not the server.");
			return;
		}

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
			cardScript.SetState(CardState.Hand);

			// Ensure RpcShowCard is called on the server
			RpcShowCard(drawnCard, CardState.Hand);

			cardList.RemoveAt(randomIndex);
		}
	}

	[ClientRpc] // Server asks client(s) to do something
	void RpcShowCard(GameObject card, CardState state)
	{
		if (state == CardState.Hand) // from drawing card
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
		else if (state == CardState.Placed) // from dropping onto drop zone
		{
			if (isOwned)
			{
				Debug.Log("Yours");
			}
			else 
			{
				Debug.Log("Not Yours");
			}
		}
	}

	[Command]
	public void CmdDropCard(GameObject card, CardState state)
	{
		RpcShowCard(card, state);
	}
}

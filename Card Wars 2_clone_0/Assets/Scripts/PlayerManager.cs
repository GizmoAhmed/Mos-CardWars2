using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static Card;
using UnityEngine.UIElements;

public class PlayerManager : NetworkBehaviour
{
	public GameObject Hand1;
	public GameObject Hand2;

	public GameObject ThisPlayer;
	public GameObject OtherPlayer;

	public List<GameObject> Cards1;
	public List<GameObject> Cards2;

	public override void OnStartClient()
	{
		base.OnStartClient();

		Hand1 = GameObject.Find("Hand1");
		Hand2 = GameObject.Find("Hand2");

		ThisPlayer = GameObject.Find("ThisPlayer");
		OtherPlayer = GameObject.Find("OtherPlayer");
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
			RpcShowCard(drawnCard, CardState.Hand, null);

			cardList.RemoveAt(randomIndex);
		}
	}

	[ClientRpc] // Server asks client(s) to do something
	void RpcShowCard(GameObject card, CardState state, GameObject land)
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
				card.GetComponent<CardFlipper>().Flip();
			}
		}
		else if (state == CardState.Placed) // from dropping onto drop zone
		{
			if (!isOwned)
			{
				card.GetComponent<CardFlipper>().Flip();

				if (land != null)
				{
					Land landScript = land.GetComponent<Land>();
					GameObject acrossLand = landScript._Across;

					card.transform.SetParent(acrossLand.transform, true);
					card.transform.localPosition = Vector2.zero;
				}
				else
				{
					Debug.LogWarning("Land is not assigned...");
				}
			}
			else 
			{
				Debug.Log("Yours");
			}
		}
	}

	[Command] // called by clients but executed on the server.
	public void CmdDropCard(GameObject card, CardState state, GameObject land)
	{
		Debug.Log("Set myLand on the server"); 

		Card cardScript = card.GetComponent<Card>();
		cardScript.myLand = land;

		// Call the RPCs
		if (state == CardState.Placed) 
		{
			RpcSetMyLand(card, land);
		}

		RpcShowCard(card, state, land);
	}

	[ClientRpc] // called by the server and executed on all clients.
	void RpcSetMyLand(GameObject card, GameObject land)
	{
		Card cardScript = card.GetComponent<Card>();
		cardScript.myLand = land;
	}

}

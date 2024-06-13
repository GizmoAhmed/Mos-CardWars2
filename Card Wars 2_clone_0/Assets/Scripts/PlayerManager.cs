using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static Card;
using UnityEngine.UIElements;

public class PlayerManager : NetworkBehaviour
{
	[Header("Hands")]
	public GameObject Hand1;
	public GameObject Hand2;

	[Header("Decks")]
	public List<GameObject> Cards1;
	public List<GameObject> Cards2;

	[Header("Magic")]
	public GameObject ThisMagic;
	public GameObject OtherMagic;

	public override void OnStartClient()
	{
		base.OnStartClient();

		Hand1 = GameObject.Find("Hand1");
		Hand2 = GameObject.Find("Hand2");

		ThisMagic = GameObject.Find("ThisMagic");
		OtherMagic = GameObject.Find("OtherMagic");
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
					CreatureLand landScript = land.GetComponent<CreatureLand>();
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
				Debug.Log("You placed this card...");
			}
		}
	}

	/* 
	 * called by clients but executed on the server.
	 * ie: a client called into drop card, now server will call rpcs
	 */
	[Command] 
	public void CmdDropCard(GameObject card, CardState state, GameObject land)
	{
		Card cardScript = card.GetComponent<Card>();
		cardScript.MyLand = land;

		if (state == CardState.Placed) 
		{
			RpcSetMyLand(card, land);
		}

		RpcShowCard(card, state, land);
	}

	[Command]
	public void CmdUpdateMagic(int Magic) 
	{
		if (isOwned)
		{
			Debug.Log("Your Magic:" + Magic.ToString());
		}
		else 
		{
			Debug.Log("Not Your Magic: " + Magic.ToString());
		}
	}

	/*
	 * called by the server and executed on all clients.
	 * ie: all clients will see that this card is given this land
	 */
	[ClientRpc] 
	void RpcSetMyLand(GameObject card, GameObject land)
	{
		Card cardScript = card.GetComponent<Card>();
		cardScript.MyLand = land;
		cardScript.currentState = CardState.Placed;
	}
}

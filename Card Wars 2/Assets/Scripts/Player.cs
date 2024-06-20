using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class Player : NetworkBehaviour
{
	[Header("Decks")]
	public List<GameObject> Deck;

	[Header("Magic and Money")]
	[SyncVar] public int Magic;
	[SyncVar] public int Money;

	public GameObject ThisHand;
	public GameObject OtherHand;
	public GameObject ThisMagicUI;
	public GameObject OtherMagicUI;
	public GameObject ThisMoneyUI;
	public GameObject OtherMoneyUI;

	private GameManager gameManager;

	public override void OnStartClient()
	{
		base.OnStartClient();

		// Manually assign hand regions through the Inspector or through script
		if (isOwned)
		{
			// This is "this" player's region
			ThisHand =		GameObject.Find("ThisPlayerHand");
			ThisMagicUI =	GameObject.Find("ThisMagic");
			ThisMoneyUI =	GameObject.Find("ThisMoney");

			// Find the other player's regions
			OtherHand =		GameObject.Find("OtherPlayerHand");
			OtherMagicUI =	GameObject.Find("OtherMagic");
			OtherMoneyUI =	GameObject.Find("OtherMoney");
		}
		else
		{
			// This is the "other" player's region
			ThisHand =		GameObject.Find("OtherPlayerHand");
			ThisMagicUI =	GameObject.Find("OtherMagic");
			ThisMoneyUI =	GameObject.Find("OtherMoney");

			// Find this player's regions
			OtherHand =		GameObject.Find("ThisPlayerHand");
			OtherMagicUI =	GameObject.Find("ThisMagic");
			OtherMoneyUI =	GameObject.Find("ThisMoney");
		}

		gameManager = FindObjectOfType<GameManager>();

		if (isOwned)
		{
			gameManager.RegisterPlayer(this);
		}
	}

	[Command]
	public void CmdDrawCard()
	{
		if (Deck.Count > 0)
		{
			int randomIndex = Random.Range(0, Deck.Count);
			GameObject cardPrefab = Deck[randomIndex];

			GameObject drawnCard = Instantiate(cardPrefab);
			NetworkServer.Spawn(drawnCard, connectionToClient);

			Card cardScript = drawnCard.GetComponent<Card>();
			cardScript.SetState(Card.CardState.Hand);

			TargetShowCard(connectionToClient, drawnCard, Card.CardState.Hand);

			Deck.RemoveAt(randomIndex);
		}
	}

	[TargetRpc]
	void TargetShowCard(NetworkConnection target, GameObject card, Card.CardState state)
	{
		if (state == Card.CardState.Hand)
		{
			if (isOwned)
			{
				card.transform.SetParent(ThisHand.transform, false);
			}
			else
			{
				card.transform.SetParent(OtherHand.transform, false);
				card.GetComponent<CardFlipper>().Flip();
			}
		}
	}

	[Command]
	public void CmdDropCard(GameObject card, Card.CardState state, GameObject land)
	{
		Card cardScript = card.GetComponent<Card>();
		cardScript.MyLand = land;

		if (state == Card.CardState.Placed)
		{
			RpcSetMyLand(card, land);
		}

		RpcShowCard(card, state, land);
	}

	[ClientRpc]
	void RpcShowCard(GameObject card, Card.CardState state, GameObject land)
	{
		Card cardScript = card.GetComponent<Card>();
		if (state == Card.CardState.Hand)
		{
			if (isOwned)
			{
				card.transform.SetParent(ThisHand.transform, false);
			}
			else
			{
				card.transform.SetParent(OtherHand.transform, false);
				card.GetComponent<CardFlipper>().Flip();
			}
		}
		else if (state == Card.CardState.Placed)
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
		}
	}

	[ClientRpc]
	public void RpcSetMyLand(GameObject card, GameObject land)
	{
		Card cardScript = card.GetComponent<Card>();
		cardScript.MyLand = land;
		cardScript.currentState = Card.CardState.Placed;
	}

	[Command]
	public void CmdUpdateMagic(int newMagic)
	{
		Magic = newMagic;
		RpcUpdateMagic(newMagic);
	}

	[ClientRpc]
	void RpcUpdateMagic(int newMagic)
	{
		if (isOwned)
		{
			ThisMagicUI.GetComponent<Magic>().ShowMagic(newMagic);
		}
		else
		{
			OtherMagicUI.GetComponent<Magic>().ShowMagic(newMagic);
		}
	}

	[Command]
	public void CmdUpdateMoney(int newMoney)
	{
		Money = newMoney;
		RpcUpdateMoney(newMoney);
	}

	[ClientRpc]
	void RpcUpdateMoney(int newMoney)
	{
		if (isOwned)
		{
			ThisMoneyUI.GetComponent<Money>().ShowMoney(newMoney);
		}
		else
		{
			OtherMoneyUI.GetComponent<Money>().ShowMoney(newMoney);
		}
	}

	[TargetRpc]
	public void TargetStartTurn(NetworkConnection target, int turn, GameManager.Phase phase)
	{
		// Handle the start of the turn for this player
	}

	[TargetRpc]
	public void TargetStartPhase(NetworkConnection target, GameManager.Phase phase)
	{
		// Handle the start of the phase for this player
	}

	[Command]
	public void CmdReadyForNextPhase()
	{
		gameManager.CmdReadyForNextPhase(this);
	}
}

using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static Card;
using TMPro;

public class Player : NetworkBehaviour
{
	public bool local;
	public bool myTurn;

	private GameObject Hand1;
	private GameObject Hand2;

	[Header("Decks")]
	public List<GameObject> Cards1;
	public List<GameObject> Cards2;

	[HideInInspector] public GameObject ThisMagic;
	[HideInInspector] public GameObject OtherMagic;

	[HideInInspector] public GameObject ThisMoney;
	[HideInInspector] public GameObject OtherMoney;

	private TextMeshProUGUI turnText;

	public override void OnStartClient()
	{
		base.OnStartClient();

		Hand1 = GameObject.Find("Hand1");
		Hand2 = GameObject.Find("Hand2");

		ThisMagic = GameObject.Find("ThisMagic");
		OtherMagic = GameObject.Find("OtherMagic");

		ThisMoney = GameObject.Find("ThisMoney");
		OtherMoney = GameObject.Find("OtherMoney");

		turnText = GameObject.Find("TurnText").GetComponent<TextMeshProUGUI>();

		GameManager game = FindAnyObjectByType<GameManager>();

		local = isOwned;

		game.playersInLobby.Add(this);

		if (isServer) 
		{
			Debug.Log($"Player {connectionToClient.connectionId} has joined.");

			game.CheckFullLobby();
		}
	}

	[Server]
	public override void OnStartServer()
	{
		base.OnStartServer();
	}

	[Command] // Client asks server to do something
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
		}
	}

	[Command] // Client asks server to do something
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

	[Command] // Client asks server to do something
	public void CmdUpdateMagic(int magic)
	{
		// Call the Rpc method to update magic for all clients
		RpcUpdateMagic(magic);
	}

	[Command]
	public void CmdUpdateMoney(int money) 
	{
		RpcUpdateMoney(money);

	}

	[ClientRpc] // server does something on all clients
	public void RpcUpdateMagic(int magic)
	{
		if (isOwned)
		{
			Magic thisMagicScript = ThisMagic.GetComponent<Magic>();
			thisMagicScript.ShowMagic(magic);
		}
		else
		{
			Magic otherMagicScript = OtherMagic.GetComponent<Magic>();
			otherMagicScript.ShowMagic(magic);
		}
	}

	[ClientRpc] // server does something on for all clients
	public void RpcUpdateMoney(int money) 
	{
		Money MoneyScript;

		if (isOwned)
		{
			MoneyScript = ThisMoney.GetComponent<Money>();
		}
		else
		{
			MoneyScript = OtherMoney.GetComponent<Money>();
		}

		MoneyScript.ShowMoney(money);
	}

	[ClientRpc]
	void RpcSetMyLand(GameObject card, GameObject land)
	{
		Card cardScript = card.GetComponent<Card>();
		cardScript.MyLand = land;
		cardScript.currentState = CardState.Placed;
	}

	[Command]
	public void CmdSetReady() 
	{
		GameManager game = FindAnyObjectByType<GameManager>();
		game.PlayerReady(connectionToClient);
	}

	[ClientRpc]
	public void RpcUpdateTurnText(int turn) 
	{
		if (turnText != null)
		{
			turnText.text = "Turn: " + turn;
		}
	}

	[Command]
	public void CmdColorTheLand(CreatureLand land, CreatureLand.Element element) 
	{
		RpcColorTheLand(land, element);
	}

	[ClientRpc]
	public void RpcColorTheLand(CreatureLand land, CreatureLand.Element element) 
	{
		if (isOwned)
		{
			land.ChangeElement(element);
		}
		else 
		{
			CreatureLand acrossScript = land._Across.GetComponent<CreatureLand>();

			acrossScript.ChangeElement(element);
		}
	}
}

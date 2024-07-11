using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static Card;
using TMPro;

public class Player : NetworkBehaviour
{
	public bool local;
	[SyncVar] public bool canPlay = true;
	[SyncVar] public bool myTurn;

	private GameObject Hand1;
	private GameObject Hand2;

	[Header("Decks")]
	public List<GameObject> Cards1;
	public List<GameObject> Cards2;

	[Header("Consumables")]
	[SyncVar(hook = nameof(RpcShowMagic))] public int Magic;
	[SyncVar(hook = nameof(RpcShowMoney))] public int Money;
	[SyncVar(hook = nameof(RpcShowCost))] public int Cost;

	private GameObject ThisMagic;
	private GameObject OtherMagic;

	private GameObject ThisMoney;
	private GameObject OtherMoney;

	private GameObject CostTextObject;

	private TextMeshProUGUI turnText;

	[Tooltip("List for battle-ready cards")]
	public List<GameObject> myBattleReadyCards = new List<GameObject>();

	public override void OnStartClient()
	{
		base.OnStartClient();

		canPlay = true;

		Hand1 = GameObject.Find("Hand1");
		Hand2 = GameObject.Find("Hand2");

		ThisMagic = GameObject.Find("ThisMagic");
		OtherMagic = GameObject.Find("OtherMagic");

		ThisMoney = GameObject.Find("ThisMoney");
		OtherMoney = GameObject.Find("OtherMoney");

		turnText = GameObject.Find("TurnText").GetComponent<TextMeshProUGUI>();

		GameManager game = FindAnyObjectByType<GameManager>();

		local = isOwned;

		myTurn = true;

		if (isServer) 
		{
			Debug.Log($"Player {connectionToClient.connectionId} has joined.");

			Player freshPlayer = connectionToClient.identity.GetComponent<Player>(); ;

			freshPlayer.Magic = freshPlayer.Money = 0; freshPlayer.Cost = 2;

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
				card.GetComponent<Card>().Ally = true;
			}
			else
			{
				card.transform.SetParent(Hand2.transform, false);
				card.GetComponent<CardFlipper>().Flip();
				card.GetComponent<Card>().Ally = false;
			}
		}
		else if (state == CardState.Placed) // from dropping onto drop zone
		{
			if (isOwned)
			{
				// handled in Card.PlaceCard(),
				// look into making PlaceCard() do the work of this section via boolean parameter of isOwned
			}
			else 
			{
				card.GetComponent<CardFlipper>().Flip();

				if (land != null)
				{
					CreatureLand landScript = land.GetComponent<CreatureLand>();
					GameObject acrossLand = landScript._Across;

					CreatureLand ScriptAcross = acrossLand.GetComponent<CreatureLand>();
					ScriptAcross.AttachCard(card);

					card.transform.SetParent(acrossLand.transform, true);
					card.transform.localPosition = Vector2.zero;

					card.GetComponent<Card>().Ally = false;
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

	[ClientRpc]
	public void RpcShowMagic(int oldMagic, int newMagic) 
	{
		TextMeshProUGUI magicText;

		if (isOwned)
		{
			magicText = ThisMagic.GetComponent<TextMeshProUGUI>();
			
		}
		else 
		{
			magicText = OtherMagic.GetComponent<TextMeshProUGUI>();
		}

		magicText.text = newMagic.ToString();
	}

	[ClientRpc]
	public void RpcShowMoney(int oldMoney, int newMoney)
	{
		TextMeshProUGUI moneyText;

		if (isOwned)
		{
			moneyText = ThisMoney.GetComponent<TextMeshProUGUI>();
		}
		else
		{
			moneyText = OtherMoney.GetComponent<TextMeshProUGUI>();
		}

		moneyText.text = newMoney.ToString();
	}

	[ClientRpc]
	public void RpcShowCost(int oldCost, int newCost)
	{
		CostTextObject = GameObject.Find("CostText");

		TextMeshProUGUI costText = CostTextObject.GetComponent<TextMeshProUGUI>();

		costText.text = newCost.ToString();
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
	public void CmdColorTheLand(CreatureLand land, CreatureLand.LandElement element) 
	{
		RpcColorTheLand(land, element);
	}

	[ClientRpc]
	public void RpcColorTheLand(CreatureLand land, CreatureLand.LandElement element) 
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

	[ClientRpc]
	public void RpcEnablePlayer(bool set) 
	{
		if (!isOwned) { return; }

		Card[] cards = FindObjectsOfType<Card>();

		foreach (Card card in cards)
		{
			card.Movable = set;
		}

		UnityEngine.UI.Button[] buttons = FindObjectsOfType<UnityEngine.UI.Button>();
		foreach (UnityEngine.UI.Button button in buttons)
		{
			button.interactable = set;
		}

		canPlay = set;
	}

	// this is simply a function meant for debugging.
	[ClientRpc]
	public void RpcTurnMessage(bool isTurn)
	{
		if (!isOwned) { return; }

		myTurn = isTurn;
		Debug.Log(isTurn ? "Enabled" : "Disabled");
	}

	[ClientRpc]
	public void RpcFindBattleCards()
	{
		if (!isOwned) { return; } // this guy just makes sure each player calls it indidually: If the server wasn't talking about you, don't run

		myBattleReadyCards.Clear();

		CreatureCard[] allCreatureCards = FindObjectsOfType<CreatureCard>();

		foreach (CreatureCard creatureCard in allCreatureCards)
		{
			if (creatureCard.Ally && creatureCard.currentState == CardState.Placed)
			{
				myBattleReadyCards.Add(creatureCard.gameObject);
			}
		}

		foreach (GameObject cardOBJ in myBattleReadyCards)
		{
			CreatureCard thisCard = cardOBJ.GetComponent<CreatureCard>();

			if (thisCard.MyLand == null) 
			{
				Debug.LogWarning(thisCard.Name + " has no land");
				return;
			}

			CreatureLand thisLand = thisCard.MyLand.GetComponent<CreatureLand>();

			CreatureLand acrossLand = thisLand._Across.GetComponent<CreatureLand>();

			if (acrossLand.CurrentCard == null)
			{
				Debug.Log(thisCard.Name + " has no one across");
			}
			else
			{
				CreatureCard acrossCard = acrossLand.CurrentCard.GetComponent<CreatureCard>();

				Debug.Log(thisCard.Name + " just attacked " + acrossCard.Name);
			}
		}
	}

}
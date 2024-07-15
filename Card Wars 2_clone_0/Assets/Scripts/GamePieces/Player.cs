using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static Card;
using TMPro;
using System.Linq;

public class Player : NetworkBehaviour
{
	public bool local;
	[SyncVar] public bool canPlay = true;
	[SyncVar] public bool myTurn;

	private GameObject Hand1;
	private GameObject Hand2;

	/*[Header("Decks")]
	public List<GameObject> Cards1;
	public List<GameObject> Cards2;*/
	public Deck deck;

	[Header("Consumables")]
	[SyncVar] public int Magic;
	[SyncVar] public int Money;
	[SyncVar] public int Cost;

	private GameObject ThisMagic;
	private GameObject OtherMagic;

	private GameObject ThisMoney;
	private GameObject OtherMoney;

	private GameObject CostTextObject;

	private TextMeshProUGUI turnText;

	public Dictionary<GameObject, int> myBattleReadyCards = new Dictionary<GameObject, int>();
	public List<GameObject> sortedBattleReadyCards;

	public override void OnStartClient()
	{
		base.OnStartClient();

		canPlay = true;

		deck = GetComponentInChildren<Deck>();

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

		CmdShowConsumable(0, "magic");
		CmdShowConsumable(0, "money");
		CmdShowConsumable(2, "cost");

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

	[ClientRpc] // Server asks client(s) to do something
	public void RpcShowCard(GameObject card, CardState state, GameObject land)
	{
		Debug.Log("Finding way to show cards...");

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

	[Command]
	public void CmdShowConsumable(int newAmount, string mode) 
	{
		RpcShowConsumable(newAmount, mode);
	}

	[ClientRpc]
	public void RpcShowConsumable(int newAmount, string mode) 
	{
		switch (mode) 
		{
			case "magic":

				Magic = newAmount;

				TextMeshProUGUI magicText;

				if (isOwned)
				{
					magicText = ThisMagic.GetComponent<TextMeshProUGUI>();
				}
				else
				{
					magicText = OtherMagic.GetComponent<TextMeshProUGUI>();
				}

				magicText.text = Magic.ToString();

				break;

			case "money":

				Money = newAmount;

				TextMeshProUGUI moneyText;

				if (isOwned)
				{
					moneyText = ThisMoney.GetComponent<TextMeshProUGUI>();
				}
				else
				{
					moneyText = OtherMoney.GetComponent<TextMeshProUGUI>();
				}

				moneyText.text = Money.ToString();

				break;

			case "cost":

				Cost = newAmount;

				CostTextObject = GameObject.Find("CostText");

				TextMeshProUGUI costText = CostTextObject.GetComponent<TextMeshProUGUI>();

				costText.text = Cost.ToString();
				break;
		}
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
	}

	[TargetRpc] // server tells a specific client do something.
	public void RpcFindBattleCards()
	{
		Debug.Log("This client is finding battle cards");

		myBattleReadyCards.Clear();
		sortedBattleReadyCards.Clear();

		CreatureCard[] allCreatureCards = FindObjectsOfType<CreatureCard>();

		foreach (CreatureCard creatureCard in allCreatureCards)
		{
			if (creatureCard.isOwned && creatureCard.currentState == CardState.Placed)
			{
				string landName = creatureCard.MyLand.name;
				int landNumber = int.Parse(landName.Substring(landName.Length - 1));

				myBattleReadyCards.Add(creatureCard.gameObject, landNumber);
			}
		}

		if (myBattleReadyCards.Count == 0) // no cards on the field 
		{
			Debug.Log("No Cards on the field for me to attack with...");
			return;
		}

		sortedBattleReadyCards = myBattleReadyCards.OrderBy(pair => pair.Value).Select(pair => pair.Key).ToList();

		foreach (GameObject cardOBJ in sortedBattleReadyCards)
		{
			CreatureCard thisCard = cardOBJ.GetComponent<CreatureCard>();

			CreatureLand thisLand = thisCard.MyLand.GetComponent<CreatureLand>();

			CreatureLand acrossLand = thisLand._Across.GetComponent<CreatureLand>();

			if (acrossLand.CurrentCard == null)
			{
				CmdAltercation(thisCard , null);
			}
			else 
			{
				CmdAltercation(thisCard , acrossLand.CurrentCard.GetComponent<CreatureCard>());
			}
		}
	}

	[Command]
	public void CmdAltercation(CreatureCard attackingCard, CreatureCard defendingCard) { FindAnyObjectByType<Combat>().Altercation(attackingCard, defendingCard); }
}
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static Card;
using TMPro;
using System.Linq;

// ternary operator
// variable = (condition) ? expressionTrue :  expressionFalse;

public class Player : NetworkBehaviour
{
	[Header("Player Traits")]
	public bool local;
	public bool canPlay = true;
	public bool myTurn;

	private GameObject Hand1; 
	private GameObject Hand2;

	[Header("Deck & Discard")]
	public Deck			deck;
	public DiscardBoard	discard;

	[Header("Magic")]
	public int CurrentMagic;
	public int MaxMagic;

	[Header("Money")]
	public int Money;
	public int DrawCost;	
	public int UpgradeCost;

	[Header("Health")]
	public int Health;

	private GameObject ThisMagic;
	private GameObject OtherMagic;

	private GameObject ThisMoney;
	private GameObject OtherMoney;

	private TextMeshProUGUI turnText;

	public List<GameObject> sortedBattleReadyCards;

	public override void OnStartClient()
	{
		base.OnStartClient();

		canPlay = true;

		deck	= GetComponentInChildren<Deck>();

		// FindObjectOfType<Script>(bool includeInactive)
		discard = FindObjectOfType<DiscardBoard>(true);

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

		CmdShowStats(0, "current_magic");
		CmdShowStats(0, "max_magic");
		CmdShowStats(0, "money");
		CmdShowStats(2, "draw_cost");		// starting cost
		CmdShowStats(1, "upgrade_cost");   // starting cost

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

		if (state == CardState.Placed)
		{
			RpcSetLand(card, land);
		}

		RpcShowCard(card, state, land);
	}

	[Command]
	public void CmdShowStats(int newAmount, string mode) { RpcShowStats(newAmount, mode); }

	[ClientRpc]
	public void RpcShowStats(int newAmount, string mode) 
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

		switch (mode) 
		{
			case "max_magic":

				MaxMagic = newAmount;

				magicText.text = CurrentMagic.ToString() + "/" + MaxMagic.ToString();

				break;

			case "current_magic":

				CurrentMagic = newAmount;

				magicText.text = CurrentMagic.ToString() + "/" + MaxMagic.ToString();

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

				moneyText.text = newAmount.ToString();

				break;

			case "draw_cost":

				GameObject DrawCostTextObject;

				if (isOwned)
				{
					DrawCost = newAmount;

					DrawCostTextObject = GameObject.Find("ThisDrawCostText");
				}
				else 
				{
					DrawCostTextObject = GameObject.Find("OtherDrawCostText");
				}

				TextMeshProUGUI drawCostText = DrawCostTextObject.GetComponent<TextMeshProUGUI>();

				drawCostText.text = newAmount.ToString();

				break;

			case "upgrade_cost":

				GameObject UpgradeCostTextObject;

				if (isOwned)
				{
					UpgradeCost = newAmount;

					UpgradeCostTextObject = GameObject.Find("ThisUpgradeCostText");
				}
				else
				{
					UpgradeCostTextObject = GameObject.Find("OtherUpgradeCostText");
				}

				TextMeshProUGUI upgradeCostText = UpgradeCostTextObject.GetComponent<TextMeshProUGUI>();

				upgradeCostText.text = newAmount.ToString();

				break;

			case "health":

				GameObject healthObj;

				if (isOwned)
				{
					Health = newAmount;

					healthObj = GameObject.Find("ThisHealth");
				}
				else
				{
					healthObj = GameObject.Find("OtherHealth");
				}

				healthObj.GetComponent<TextMeshProUGUI>().text = newAmount.ToString();

				break;

			default:
				Debug.LogError("forgot mode in ShowStats");
				break;
		}
	}

	[ClientRpc]
	void RpcSetLand(GameObject card, GameObject land)
	{
		Card cardScript = card.GetComponent<Card>();

		if (isOwned)
		{
			land.GetComponent<CreatureLand>().AttachCard(card);
		}
		else 
		{
			GameObject across = land.GetComponent<CreatureLand>().across;
			across.GetComponent<CreatureLand>().AttachCard(card);
		}
		
		cardScript.currentState = CardState.Placed;
	}

	[Command]
	public void CmdSetReady() { FindAnyObjectByType<GameManager>().PlayerReady(connectionToClient); }

	[ClientRpc]
	public void RpcUpdateTurnText(int turn) 
	{
		turnText.text = "TURN: " + turn;
	}

	[Command]
	public void CmdColorTheLand(CreatureLand land, CreatureLand.LandElement element) { RpcColorTheLand(land, element); }

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

	[TargetRpc]
	public void RpcEnablePlayer(bool set) 
	{		
		Card[] cards = FindObjectsOfType<Card>();

		foreach (Card card in cards) { card.Movable = set; }

		UnityEngine.UI.Button[] buttons = FindObjectsOfType<UnityEngine.UI.Button>();

		foreach (UnityEngine.UI.Button button in buttons) { button.interactable = set; }

		myTurn = canPlay = set;
	}

	[TargetRpc] // server tells a specific client do something. (ie player0.RpcFindBattleCards)
	public void RpcFindBattleCards()
	{
		sortedBattleReadyCards.Clear();

		CreatureCard[] allCreatureCards = FindObjectsOfType<CreatureCard>();

		Dictionary<GameObject, int> myBattleReadyCards = new Dictionary<GameObject, int>();

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
	public void CmdAltercation(CreatureCard attackingCard, CreatureCard defendingCard)
	{ FindAnyObjectByType<Combat>().Altercation(attackingCard, defendingCard); }
}
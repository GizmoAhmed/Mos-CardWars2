using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static Card;
using TMPro;
using System.Linq;
using System.Collections;

// ternary operator
// variable = (condition) ? expressionTrue :  expressionFalse;

public class Player : NetworkBehaviour
{
	public bool canPlay = true;
	public bool myTurn;

	[Header("Deck & Discard")]
	public Deck deck;
	public DiscardBoard discard;

	[Header("Magic")]
	public int CurrentMagic;
	public int MaxMagic;

	[Header("Money")]
	public int Money;
	public int DrawCost;
	public int UpgradeCost;

	[Header("Score")]
	public int Score;

	[Header("Health")]
	public int Health;

	// Find once instead of multiple times
	//***********************************************************************
	private GameManager	gameManager;
	private Combat		combat;

	private GameObject Hand1;
	private GameObject Hand2;

	private GameObject ThisMagic;
	private GameObject OtherMagic;

	private GameObject ThisMoney;
	private GameObject OtherMoney;

	private TextMeshProUGUI turnText;
	//***********************************************************************

	[Header("Battle Ready Cards")]
	[SyncVar] public bool searchComplete;

	public override void OnStartClient()
	{
		base.OnStartClient();

		canPlay = true;

		deck = GetComponentInChildren<Deck>();

		// FindObjectOfType<Script>(bool includeInactive)
		discard = FindObjectOfType<DiscardBoard>(true);

		Hand1 = GameObject.Find("Hand1");
		Hand2 = GameObject.Find("Hand2");

		ThisMagic = GameObject.Find("ThisMagic");
		OtherMagic = GameObject.Find("OtherMagic");

		ThisMoney = GameObject.Find("ThisMoney");
		OtherMoney = GameObject.Find("OtherMoney");

		turnText = GameObject.Find("TurnText").GetComponent<TextMeshProUGUI>();

		gameManager = FindAnyObjectByType<GameManager>();

		combat = FindAnyObjectByType<Combat>();

		myTurn = true;

		CmdChangeStats(0, "current_magic");
		CmdChangeStats(0, "max_magic");

		CmdChangeStats(0, "money");
		CmdChangeStats(2, "draw_cost");       
		CmdChangeStats(1, "upgrade_cost");

		CmdChangeStats(0, "score");

		// CmdChangeStats(75, "health"); already set in gameManager.IdentifyPlayers()

		if (isServer)
		{
			Debug.Log($"Player {connectionToClient.connectionId} has joined.");
			gameManager.CheckFullLobby();
		}
	}

	[Server]
	public override void OnStartServer()
	{
		base.OnStartServer();
	}

	[Command]
	public void CmdSetReady()
	{
		gameManager.PlayerReady(connectionToClient);
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

	[Command]
	public void CmdDropCard(GameObject card, CardState state, GameObject land)
	{
		RpcHandleCard(card, state, land);
	}

	[ClientRpc]
	public void RpcHandleCard(GameObject card, CardState state, GameObject land)
	{
		card.GetComponent<Card>().SetState(state); // make sure state is shown on all clients

		if (state == CardState.Hand) // drawing card from deck
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
				land.GetComponent<CreatureLand>().AttachCard(card);
			}
			else
			{
				card.GetComponent<CardFlipper>().Flip();

				if (land != null)
				{
					CreatureLand landScript = land.GetComponent<CreatureLand>();
					GameObject acrossLand = landScript._Across;

					acrossLand.GetComponent<CreatureLand>().AttachCard(card);

					card.transform.SetParent(acrossLand.transform, true);
					card.transform.localPosition = Vector2.zero;
				}
			}
		}
	}


	[Command]
	public void CmdChangeStats(int newAmount, string stat)
	{
		RpcShowStats(newAmount, stat);
	}

	[ClientRpc]
	public void RpcShowStats(int newAmount, string stat) 
	{
		switch (stat) 
		{
			case "max_magic":

				MaxMagic = newAmount;

				TextMeshProUGUI magicText = (isOwned) ? ThisMagic.GetComponent<TextMeshProUGUI>() :  OtherMagic.GetComponent<TextMeshProUGUI>();
				
				magicText.text = CurrentMagic.ToString() + "/" + MaxMagic.ToString();

				break;

			case "current_magic":

				CurrentMagic = newAmount;

				magicText = (isOwned) ? ThisMagic.GetComponent<TextMeshProUGUI>() : OtherMagic.GetComponent<TextMeshProUGUI>();

				magicText.text = CurrentMagic.ToString() + "/" + MaxMagic.ToString();

				break;

			case "money":

				Money = newAmount;

				TextMeshProUGUI moneyText = (isOwned) ? ThisMoney.GetComponent<TextMeshProUGUI>() :  OtherMoney.GetComponent<TextMeshProUGUI>();

				moneyText.text = newAmount.ToString();

				break;

			case "draw_cost":

				DrawCost = newAmount;

				GameObject DrawCostTextObject = (isOwned) ? GameObject.Find("ThisDrawCostText") :  GameObject.Find("OtherDrawCostText");

				TextMeshProUGUI drawCostText = DrawCostTextObject.GetComponent<TextMeshProUGUI>();

				drawCostText.text = newAmount.ToString();

				break;

			case "upgrade_cost":

				UpgradeCost = newAmount;

				GameObject UpgradeCostTextObject = (isOwned) ? GameObject.Find("ThisUpgradeCostText") :  GameObject.Find("OtherUpgradeCostText");

				TextMeshProUGUI upgradeCostText = UpgradeCostTextObject.GetComponent<TextMeshProUGUI>();

				upgradeCostText.text = newAmount.ToString();

				break;

			case "health":

				Health = newAmount;

				GameObject healthObj = (isOwned) ? GameObject.Find("ThisHealth") : GameObject.Find("OtherHealth");

				healthObj.GetComponent<TextMeshProUGUI>().text = newAmount.ToString();

				break;

			case "score":

				Score = newAmount;

				GameObject scoreObj = (isOwned) ? GameObject.Find("ThisScore") : GameObject.Find("OtherScore");

				scoreObj.GetComponent<TextMeshProUGUI>().text = newAmount.ToString();

				break;

			default:
				Debug.LogError(stat + " is a invalid");
				break;
		}
	}

	[ClientRpc]
	public void RpcUpdateTurnText(int turn) 
	{
		turnText.text = "TURN: " + turn;
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

	[TargetRpc]
	public void TargetStartBattleCardsSearch(NetworkConnection target)
	{
		StartCoroutine(FindBattleCardsCoroutine());
	}

	private IEnumerator FindBattleCardsCoroutine()
	{
		/// C# does not have list comprehensions like Python, 
		/// but it has LINQ (Language Integrated Query) which provides a similar capability. 
		/// You can use LINQ to filter and project data in a more concise manner.
		
		var battleReadyCards = FindObjectsOfType<CreatureCard>()
			.Where(creatureCard => creatureCard.isOwned && creatureCard.currentState == CardState.Placed)
			.Select(creatureCard => {
				string landName = creatureCard.MyLand.name;
				int landNumber = int.Parse(landName.Substring(landName.Length - 1));
				return new KeyValuePair<GameObject, int>(creatureCard.gameObject, landNumber);
			} ).ToList();

		if (battleReadyCards.Count == 0) // no cards on the field 
		{
			CmdSetSearch(true);
			yield break;
		}

		// Sort the battle-ready cards by their land number
		battleReadyCards = battleReadyCards.OrderBy(pair => pair.Value).ToList();

		foreach (var pair in battleReadyCards)
		{
			GameObject cardOBJ = pair.Key;

			CreatureCard thisCard = cardOBJ.GetComponent<CreatureCard>();
			CreatureLand thisLand = thisCard.MyLand.GetComponent<CreatureLand>();
			CreatureLand acrossLand = thisLand._Across.GetComponent<CreatureLand>();

			if (acrossLand.CurrentCard == null)
			{
				CmdAltercation(thisCard, null);
			}
			else
			{
				CmdAltercation(thisCard, acrossLand.CurrentCard.GetComponent<CreatureCard>());
			}

			yield return new WaitForSeconds(combat.combatDelay); // Pause between each iteration
		}

		CmdSetSearch(true);
	}


	[Command]
	private void CmdSetSearch(bool value)
	{
		searchComplete = value;
	}

	[Command]
	public void CmdAltercation(CreatureCard attackingCard, CreatureCard defendingCard)
	{
		combat.Altercation(attackingCard, defendingCard);
	}
}
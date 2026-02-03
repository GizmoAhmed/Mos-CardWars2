using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using System.Linq;
using System.Collections;
using Buttons;
using CardScripts;
using PlayerStuff;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine.UI;

// ternary operator
// variable = (condition) ? expressionTrue :  expressionFalse;

public class Player : NetworkBehaviour
{
	public CardHandler cardHandler;
	public PlayerStats playerStats;
	
	public bool myTurn;

	[Header("Deck & Discard")]
	public DeckCollection deckCollection; 

	private GameManager	gameManager;
	
	private TurnManager turnManager;

	[Header("Battle Ready Cards")]
	[SyncVar] public bool searchComplete;
	
	[Header("Buttons to disable")]
	public Button draw;
	public Button upgrade;
	public Button ready;

	public override void OnStartClient()
	{
		base.OnStartClient();

		turnManager = FindAnyObjectByType<TurnManager>();
		
		deckCollection		= GetComponentInChildren<DeckCollection>();
		
		cardHandler = GetComponentInChildren<CardHandler>();

		cardHandler.Init();
		
		playerStats = GetComponentInChildren<PlayerStats>();

		playerStats.InitUI();
		
		myTurn = true;

		// find buttons
		draw	= FindAnyObjectByType<DrawButton>().gameObject.GetComponent<Button>();
		upgrade = FindAnyObjectByType<UpgradeMagic>().gameObject.GetComponent<Button>();
		ready	= FindAnyObjectByType<Ready>().gameObject.GetComponent<Button>();
		
		if (isServer) // must be the host, the first person to join
		{
			
		}
		else // the joiner, joins if there is a host, they are never alone
		{
			CmdCheckFullLobby();
		}
	}

	[Command]
	private void CmdCheckFullLobby()
	{
		gameManager = FindAnyObjectByType<GameManager>();
		gameManager.FullLobby();
	}

	[Server]
	public override void OnStartServer()
	{
		base.OnStartServer();
	}

	[Command]
	public void CmdSetReady()
	{
		turnManager.PlayerReady(connectionToClient);
	}

	
	/// <summary>
	/// True for enabling, false for disabling
	/// </summary>
	/// <param name="t"></param>
	[TargetRpc]
	public void Disable(bool enable)
	{
		myTurn = draw.interactable = upgrade.interactable = ready.interactable = enable;
	}

	/*[TargetRpc]
	public void TargetStartBattleCardsSearch(NetworkConnection target)
	{
		StartCoroutine(FindBattleCardsCoroutine());
	}*/

	/// <summary>
	/// C# does not have list comprehensions like Python,
	/// but it has LINQ (Language Integrated Query) which provides a similar capability. 
	/// You can use LINQ to filter and project data in a more concise manner.
	/// </summary>
	/// <returns></returns>
	/*private IEnumerator FindBattleCardsCoroutine()
	{
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
	}*/


	[Command]
	private void CmdSetSearch(bool value)
	{
		searchComplete = value;
	}

	/*[Command]
	public void CmdAltercation(CreatureCard attackingCard, CreatureCard defendingCard)
	{
		combat.Altercation(attackingCard, defendingCard);
	}*/
}
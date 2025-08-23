using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using System.Linq;
using System.Collections;
using Unity.Collections;
using Unity.VisualScripting;

// ternary operator
// variable = (condition) ? expressionTrue :  expressionFalse;

public class Player : NetworkBehaviour
{
	public CardHandler cardHandler;
	public PlayerStats playerStats;
	
	public bool canPlay = true;
	public bool myTurn;

	[Header("Deck & Discard")]
	public Deck deck; 

	private GameManager	gameManager;

	[Header("Battle Ready Cards")]
	[SyncVar] public bool searchComplete;

	public override void OnStartClient()
	{
		base.OnStartClient();

		canPlay = true;

		deck = GetComponentInChildren<Deck>();
		
		cardHandler = GetComponentInChildren<CardHandler>();
		
		playerStats = GetComponentInChildren<PlayerStats>();

		playerStats.InitUI();

		gameManager = FindAnyObjectByType<GameManager>();
		
		myTurn = true;
		
		if (isServer)
		{
			Debug.Log($"[SERVER] >>> Player {connectionToClient.connectionId} has joined.");
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
		// gameManager.PlayerReady(connectionToClient);
	}

	/*[TargetRpc]
	public void RpcEnablePlayer(bool set)
	{
		Card[] cards = FindObjectsOfType<Card>();

		foreach (Card card in cards) { card.Movable = set; }

		UnityEngine.UI.Button[] buttons = FindObjectsOfType<UnityEngine.UI.Button>();

		foreach (UnityEngine.UI.Button button in buttons) { button.interactable = set; }

		myTurn = canPlay = set;
	}*/

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
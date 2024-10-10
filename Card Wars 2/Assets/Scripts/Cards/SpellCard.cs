using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

public class SpellCard : Card
{
	[Header("Spell Type")]

	[Tooltip("Charm is placed on indivudal cards\n" +
			" OneTimeCast is for cards that are activated then discarded\n" +
			" Status is for lasting cards that effect over time")]

	public SpellType spellType;
	public enum SpellType { Charm, OneTimeCast, Status };

	[Header("Spell Stats")]
	public int currentTime;
	public int maxTime;

	[Tooltip("Allows for spell to last an extra turn")] public bool firstTurn;

	new void Start()
	{
		base.Start();

		Transform timeTextTransform = transform.Find("Timer");

		if (timeTextTransform != null)
		{
			spellType = SpellType.Status; // there is a timer? then it lasts, call it Status
		}

		landTag = "SpellLand";

		/*if (Type == SpellType.Active)
		{
			landTag = "SpellLand";
			transform.Find("Timer").GetComponent<TextMeshProUGUI>().text = currentTime.ToString();
			firstTurn = true;
		}
		else
		{
			landTag = "TargetSpell";
			maxTime = currentTime = 0;
		}*/
	}

	[ClientRpc]
	public override void RpcDecay()
	{
		if (firstTurn || spellType == SpellType.Status)
		{
			firstTurn = false;
			return;
		}

		currentTime -= 1;

		if (currentTime <= 0)
		{
			FillTime();
			base.Discard();
		}
		else
		{
			transform.Find("Timer").GetComponent<TextMeshProUGUI>().text = currentTime.ToString();
		}
	}

	public void FillTime()
	{
		currentTime = maxTime;
		transform.Find("Timer").GetComponent<TextMeshProUGUI>().text = currentTime.ToString();
	}

	public override void AttachCharmSlot() { } // does nothing, becasue its a spell with no charm slot

	public void CastSpell(GameObject land) 
	{
		if (spellType == SpellType.Charm) 
		{
			CreatureLand landscript = land.GetComponent<CreatureLand>();
			GameObject cardOnLand = landscript.CurrentCard;
			Card cardScript = cardOnLand.GetComponent<Card>();
			GameObject activeCharmOBJ = cardScript.Charm;
			ActiveCharm charmSlot = activeCharmOBJ.GetComponent<ActiveCharm>();

			charmSlot.AttachCharm(gameObject);
		}
		else
		{

		}
	}

	public override void PlaceCard(GameObject land)
	{
		CastSpell(land);
		Discard();
	}

	public override void Discard()
	{
		SetState(CardState.Discard);

		// MyLand.GetComponent<CreatureLand>().DetachCard(gameObject); 

		CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

		Animate.FadeOut(canvasGroup, gameObject, player, isOwned);
	}

	public override bool ValidCastPlacement(CreatureLand landscript) 
	{
		// Debug.LogWarning("SpellCard called valid place");

		if (spellType == SpellType.Charm)
		{
			return landscript.Taken; // add charm to the creature that takes up the land
		}
		else 
		{
			return true; // OneTime and Status can be cast anywhere
		}
	}

}

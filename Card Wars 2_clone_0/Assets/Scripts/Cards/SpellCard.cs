using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

public class SpellCard : Card
{
	[Header("Spell Type")]

	[Tooltip("Target: Cast on single creature effects that creature and/ore lane " +
		"(i.e. Stim, Volcano, Brief Power)\r\n\r\n" +
		"Field: Cast Anywhere (creature present or not), " +
		"a one time effect (i.e. strawberry butt, enchiridion, dagger storm)\r\n\r\n" +
		"Status: Cast Anywhere (creature present or not), " +
		"leaves a lasting effect in play for a time (i.e. Tempest, Frost, Drought)  ")]

	public SpellType spellType;
	public enum SpellType { Target, Field, Status };

	[Header("Spell Stats")]
	public int currentTime;
	public int maxTime;

	[Tooltip("Allows for spell to have first turn not count towards timer")]
	public bool firstTurn;

	new void Start()
	{
		base.Start();

		Transform timeTextTransform = transform.Find("Timer");

		landTag = "SpellLand";

		if (timeTextTransform != null)
		{
			spellType = SpellType.Status; // there is a timer? then it lasts, call it Status

			transform.Find("Timer").GetComponent<TextMeshProUGUI>().text = currentTime.ToString();
			firstTurn = true;
		}
		else
		{
			maxTime = currentTime = 0;
		}
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

	public virtual void CastSpell(GameObject land) 
	{
		Debug.Log(spellType + " spell " + gameObject.name + " was cast");
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

		if (spellType == SpellType.Target)
		{
			return landscript.Taken; // add charm to the creature that takes up the land
		}
		else 
		{
			return true; // OneTime and Status can be cast anywhere
		}
	}

}

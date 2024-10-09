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

	public SpellType Type;
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
			Type = SpellType.Status; // there is a timer? then it lasts, call it Status
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
		if (firstTurn || Type == SpellType.Status)
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

	public void CastSpell(GameObject land) 
	{

	}

	public override void PlaceCard(GameObject land)
	{
		Debug.LogWarning(name + " Spell is trying to CAST");
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

public class SpellCard : Card
{
	[Header("Spell Type")]

	public SpellType Type;
	public enum SpellType { Target, Active };

	[Header("Spell Stats")]
	public int currentTime;
	public int maxTime;

	[Tooltip("Allows for spell to last an extra turn")] public bool firstTurn;

	new void Start()
	{
		base.Start();

		Transform timeTextTransform = transform.Find("Timer");

		Type = timeTextTransform != null ? SpellType.Active : SpellType.Target;

		if (Type == SpellType.Active)
		{
			landTag = "SpellLand";
			transform.Find("Timer").GetComponent<TextMeshProUGUI>().text = currentTime.ToString();
			firstTurn = true;
		}
		else
		{
			landTag = "TargetSpell";
			maxTime = currentTime = 0;
		}
	}

	[ClientRpc]
	public override void RpcDecay()
	{
		if (firstTurn || Type == SpellType.Target)
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

}

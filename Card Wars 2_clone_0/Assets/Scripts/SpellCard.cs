using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class SpellCard : Card
{
	public bool ActiveSpell;

	new void Start()
	{
		base.Start();

		Transform timeTextTransform = transform.Find("TimeText");

		ActiveSpell = timeTextTransform != null ? true : false;

		if (ActiveSpell)
		{
			landTag = "SpellLand";
		}
		else 
		{
			landTag = "CreatureLand";
		}
	}
}

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

		// Use the ternary operator to set ActiveSpell
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

	/*protected override void OnTriggerStay2D(Collider2D other)
	{
		base.OnTriggerStay2D(other);
	}*/
}

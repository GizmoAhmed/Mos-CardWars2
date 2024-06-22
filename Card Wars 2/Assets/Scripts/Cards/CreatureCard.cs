using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class CreatureCard : Card
{
	new void Start()
	{
		base.Start();
		landTag = "CreatureLand";
	}
}

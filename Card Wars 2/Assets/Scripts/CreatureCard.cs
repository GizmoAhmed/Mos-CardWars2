using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class CreatureCard : Card
{
	// attack, defense, and stars go here
	new void Start()
	{
		base.Start();
		landTag = "CreatureLand";
	}
}

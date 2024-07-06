using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class BuildingCard : Card
{

	// each building would need to check when a creatuer is placed, flooped, dies, etc.
	// add checks for both

	new void Start()
	{
		base.Start();
		landTag = "BuildLand";
	}
}

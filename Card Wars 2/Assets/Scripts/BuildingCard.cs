using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class BuildingCard : Card
{
	new void Start()
	{
		base.Start();
		landTag = "BuildLand";
	}
}

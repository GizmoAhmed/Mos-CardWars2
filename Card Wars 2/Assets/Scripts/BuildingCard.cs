using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class BuildingCard : Card
{
	void Start()
	{
		base.Start();
		landTag = "BuildLand";
	}

	protected override void OnTriggerStay2D(Collider2D other)
	{
		base.OnTriggerStay2D(other);
	}
}

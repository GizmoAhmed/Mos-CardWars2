using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// variable = (condition) ? expressionTrue :  expressionFalse;

public class SpellLand : CreatureLand
{
    [Header("Spell Land Tratis")]
    public List<GameObject> ActiveSpells;

	new void Start()
    {
		currentElement = LandElement.Null;
		InitializeNeighbors();
	}

	protected override void InitializeNeighbors()
	{
		string landName = this.gameObject.name;

		if (landName.StartsWith("p1SpellLand"))
		{
			_Across = GameObject.Find("p2SpellLand");
		}
		else if (landName.StartsWith("p2SpellLand"))
		{
			_Across = GameObject.Find("p1SpellLand");
		}
	}


	public override void AttachCard(GameObject card) 
    {
		card.GetComponent<Card>().MyLand = gameObject;
		ActiveSpells.Add(card);
	}

	public override void DetachCard(GameObject card)
	{
		card.GetComponent<Card>().MyLand = null;
		ActiveSpells.Remove(card);
	}
}

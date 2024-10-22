using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stim : SpellCard
{
	public int AttackBoost = 2;
	public int DefenseBoost = 2;

	public override void CastSpell(GameObject land)
	{
		Debug.LogWarning("Stim used on " + land.name);
	}
}

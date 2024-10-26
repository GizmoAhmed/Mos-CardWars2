using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stim : SpellCard
{
	public int AttackBoost = 2;
	public int DefenseBoost = 2;

	protected override void Start()
	{
		base.Start();
		spellType = SpellType.Target;
	}

	public override void CastSpell(GameObject land)
	{
		Debug.LogWarning("Stim used on " + land.name);
	}
}

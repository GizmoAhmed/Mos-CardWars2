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
		CreatureLand landScript = land.GetComponent<CreatureLand>();
		if (landScript == null) { return; }

		CreatureCard creature = landScript.CurrentCard.GetComponent<CreatureCard>();
		if (creature == null) { return; }

		creature.CmdBuff(AttackBoost, true, "attack");
		creature.CmdBuff(DefenseBoost, true, "defense");
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpellLand : CreatureLand
{
    public List<GameObject> ActiveSpells;

    public GameObject OtherSpellSide;

    public GridLayout grid;

    // hides the og CreatureLand start()
	new void Start()
    {
        OtherSpellSide = GameObject.Find("OtherSpellLand");
        _Across = OtherSpellSide;
	}

    public override void AttachCard(GameObject card) 
    {
		ActiveSpells.Add(card);
	}

    public void DiscardSpell(GameObject spell) 
    {
        ActiveSpells.Remove(spell);
    }
}

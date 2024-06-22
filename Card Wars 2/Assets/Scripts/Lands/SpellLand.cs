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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddSpell(GameObject spell) 
    {
        ActiveSpells.Add(spell);
    }

    public void DiscardSpell(GameObject spell) 
    {
        ActiveSpells.Remove(spell);
    }
}

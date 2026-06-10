using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardStatss;
using Newtonsoft.Json.Serialization;
using Tiles;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[CreateAssetMenu(fileName = "TradeDefenseForStrength_OnCreature_Spell", menuName = "Abilities/Spell/TradeDefenseForStrength_OnCreature_Spell")]
public class TradeDefenseForStrength_OnCreature_Spell : CastAbilitySO
{
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        GameObject creatureOnTile = GetCreatureFromEventData(eventData);
            
        if (creatureOnTile == null)
            return; // error message inside above function
            
        CreatureStats creatureStats = creatureOnTile.GetComponent<CreatureStats>();
        
        int defense = creatureStats.defense; // because of condition check below, defense should always be above 1

        int trade = defense - 1; // leaves creature with 1 defense

        // save mults
        int savedStrMult = creatureStats.strengthMult;
        int savedDefMult = creatureStats.defenseMult;

        // reset mults
        creatureStats.strengthMult = 1;
        creatureStats.defenseMult = 1;

        // trade stats
        creatureStats.ChangeCreatureDefense(trade, buff:false); // lose all defense except 1
        creatureStats.ChangeCreatureStrength(trade, buff:true); // give str all the def

        // set mults back
        creatureStats.strengthMult = savedStrMult;
        creatureStats.defenseMult = savedDefMult;
    }
    

    // only castable if creature has more than 1 defense to trade
    public override bool SpecificSpellPlacementConditions(Tile tile)
    {
        MiddleTile thisTile = tile as MiddleTile;

        if (thisTile.logicalCreature != null)
        {
            CreatureStats creatureStats = thisTile.logicalCreature.GetComponent<CreatureStats>();

            if (creatureStats.defense > 1)
            {
                return true;
            }
        }
        
        return false;
    }

}

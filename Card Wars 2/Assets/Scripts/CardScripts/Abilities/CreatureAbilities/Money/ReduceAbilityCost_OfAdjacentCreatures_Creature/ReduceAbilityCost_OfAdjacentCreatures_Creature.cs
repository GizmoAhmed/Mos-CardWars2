using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using Tiles;
using UnityEngine;

[CreateAssetMenu(
    fileName = "ReduceAbilityCost_OfAdjacentCreatures_Creature", 
    menuName = "Abilities/Creature/ReduceAbilityCost_OfAdjacentCreatures_Creature")]
public class ReduceAbilityCost_OfAdjacentCreatures_Creature : ActiveAbilitySO
{
    public int selfDamage;
    public int abilityCostReduction;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        // get adjacent
        List<MiddleTile> ajdTiles = thisCard.Ext_GetTilesAdjacentToThisCard();

        // buff each
        foreach (MiddleTile tile in ajdTiles) // only goes twice
        {
            if (tile.logicalCreature == null) continue; // if there's no creature there, do nothing

            CreatureStats adjCreature = tile.logicalCreature.GetComponent<CreatureStats>();
            
            // if creature is there, buff it
            adjCreature.abilityCost -= abilityCostReduction;
        }
        
        thisCard.GetComponent<CreatureStats>().UpdateCreatureDefense(amount: selfDamage, buff: false);

        AnimateAbilityExecute(thisCard);
    }
}
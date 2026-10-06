using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using Tiles;
using UnityEngine;

[CreateAssetMenu(
    fileName = "FortifyAdjacentCreatures_ThenDamageThis_Creature", 
    menuName = "Abilities/Creature/FortifyAdjacentCreatures_ThenDamageThis_Creature")]
public class FortifyAdjacentCreatures_ThenDamageThis_Creature : ActiveAbilitySO
{
    public int fortify;
    public int selfDamage;
        
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
            adjCreature.UpdateCreatureDefense(fortify, true);
        }
        
        thisCard.GetComponent<CreatureStats>().UpdateCreatureDefense(amount: selfDamage, buff: false);

        AnimateAbilityExecute(thisCard);
    }
}
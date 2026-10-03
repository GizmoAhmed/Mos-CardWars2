using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using Tiles;
using UnityEngine;

[CreateAssetMenu(
    fileName = "StrengthenAdjacent_Creature", 
    menuName = "Abilities/Creature/StrengthenAdjacent_Creature")]
public class StrengthenAdjacent_Creature : ActiveAbilitySO
{
    public int str;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        List<MiddleTile> ajdTiles = thisCard.Ext_GetTilesAdjacentToThisCard();
        
        foreach (MiddleTile tile in ajdTiles) // only goes twice
        {
            if (tile.logicalCreature == null) continue; // if there's no creature there, do nothing

            CreatureStats adjCreature = tile.logicalCreature.GetComponent<CreatureStats>();
            
            // if creature is there, buff it
            adjCreature.UpdateCreatureStrength(str, true);
        }
        AnimateAbilityExecute(thisCard);
    }
}
using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using Extensions;
using GameManagement;
using Tiles;
using UnityEngine;

[CreateAssetMenu(fileName = "FortifyAdjacent_Creature", menuName = "Abilities/Creature/Defense/FortifyAdjacent_Creature")]
public class FortifyAdjacent_Creature : ActiveAbilitySO
{
    public int AdjacentBuffAmount;
    
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        List<MiddleTile> ajdTiles = thisCard.Ext_GetTilesAdjacentToThisCard();
        
        foreach (MiddleTile tile in ajdTiles) // only goes twice
        {
            if (tile.logicalCreature == null) continue; // if there's no creature there, do nothing

            CreatureStats adjCreature = tile.logicalCreature.GetComponent<CreatureStats>();
            
            // if creature is there, buff it
            adjCreature.UpdateCreatureDefense(AdjacentBuffAmount, true);
        }
        AnimateAbilityExecute(thisCard); }
}

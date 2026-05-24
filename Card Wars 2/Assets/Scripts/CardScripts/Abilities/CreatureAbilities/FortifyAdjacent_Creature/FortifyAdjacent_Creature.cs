using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using GameManagement;
using Tiles;
using UnityEngine;

[CreateAssetMenu(fileName = "FortifyAdjacent_Creature", menuName = "Abilities/Creature/FortifyAdjacent_Creature")]
public class FortifyAdjacent_Creature : ActiveAbilitySO
{
    public int AdjacentBuffAmount;
    
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureMovement move =  thisCard.GetComponent<CreatureMovement>();

        MiddleTile thisTile = move.GetLogicalTile() as MiddleTile;

        if (thisTile == null) return;
        
        List<MiddleTile> ajdTiles = TileManager.Instance.GetAdjacentTiles
            (thisTile.row,thisTile.column, thisTile.serverPlayerSide);

        foreach (MiddleTile tile in ajdTiles) // only goes twice
        {
            CreatureStats adjCreature = tile.logicalCreature?.GetComponent<CreatureStats>();
            
            // if creature is there, buff it
            adjCreature?.ChangeCreatureDefense(AdjacentBuffAmount, true);
        }
    }
}

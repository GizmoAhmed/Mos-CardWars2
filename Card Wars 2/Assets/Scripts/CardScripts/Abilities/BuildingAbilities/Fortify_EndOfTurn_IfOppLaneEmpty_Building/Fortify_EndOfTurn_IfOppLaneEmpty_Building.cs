using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using GameManagement;
using Tiles;
using UnityEngine;

[CreateAssetMenu(fileName = "Fortify_EndOfTurn_IfOppLaneEmpty_Building",
    menuName = "Abilities/Building/Fortify_EndOfTurn_IfOppLaneEmpty_Building")]
public class Fortify_EndOfTurn_IfOppLaneEmpty_Building : PassiveAbilitySO
{
    public int fortify;
    
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        // Debug.Log($"{thisCard.name} ability execution called...");
        
        MiddleTile thisTile = thisCard.GetTile_Ext() as MiddleTile;

        if (thisTile.logicalCreature == null)
        {
            // no creature on this tile to even buff
            return;
        }
        
        // get across tile
        MiddleTile across = thisTile.Ext_GetTileAcrossFromThisTile() as MiddleTile;

        if (across.logicalCreature == null) // buff this creature if across is empty
        {
            thisTile.logicalCreature.GetComponent<CreatureStats>().
                ChangeCreatureDefense(fortify, buff: true);
        }
    }
}

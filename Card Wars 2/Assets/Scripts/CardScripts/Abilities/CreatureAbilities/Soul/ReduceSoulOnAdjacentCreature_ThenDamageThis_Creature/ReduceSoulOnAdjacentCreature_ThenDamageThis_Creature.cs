using System;
using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using GameManagement;
using Tiles;
using UnityEngine;

[CreateAssetMenu(fileName = "ReduceSoulOnAdjacentCreature_ThenDamageThis_Creature", menuName = "Abilities/Creature/Soul/ReduceSoulOnAdjacentCreature_ThenDamageThis_Creature")]
public class ReduceSoulOnAdjacentCreature_ThenDamageThis_Creature : ActiveAbilitySO
{
    public int selfDamage;
    public int soulReduction;

    public AdjacentSide Side;
    public enum AdjacentSide
    {   
        Left,
        Right   
    }

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        // get tile
        MiddleTile thisTile = thisCard.Ext_GetTile() as MiddleTile;

        MiddleTile adj;
        
        if (Side == AdjacentSide.Left)
        {
            adj = TileManager.Instance.GetLeftAdjacentTile(thisTile);
        }
        else
        {
            adj = TileManager.Instance.GetRightAdjacentTile(thisTile);
        }

        if (adj == null) // no tile to the left or right of this one
        {
            return;
        }
        
        // look at adjacent creature
        GameObject adjCreature = adj.logicalCreature;

        if (adjCreature == null)
        {
            return; // no creature on this tile
        }
        
        // buff that adjacent creature
        adjCreature.GetComponent<CreatureStats>().UpdateSyncSoulToPlayer(-soulReduction);
        
        // damage thisCard
        thisCard.GetComponent<CreatureStats>().UpdateCreatureDefense(amount: selfDamage, buff:false);
        
        AnimateAbilityExecute(thisCard);
    }
}

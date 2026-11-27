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

[CreateAssetMenu(fileName = "ReduceAnAdjSoul_DamageSelf_Creature",
    menuName = "Abilities/Creature/Soul/ReduceAnAdjSoul_DamageSelf_Creature")]
public class ReduceAnAdjSoul_DamageSelf_Creature : ActiveAbilitySO
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
        string leftOrRight = Side == AdjacentSide.Left ? "left" : "right";
        
        MiddleTile adj = thisCard.Ext_GetEitherAdjacentTile(leftOrRight);

        if (adj == null || adj.logicalCreature == null) // in a position where there is not right or left, or simply no creature on that tile 
        {
            thisCard.GetComponent<CreatureStats>().UpdateCreatureDefense(amount: selfDamage, buff: false); // damage this anyway
            return;
        } 
        
        // look at adjacent creature
        GameObject adjCreature = adj.logicalCreature;

        // buff that adjacent creature
        adjCreature.GetComponent<CreatureStats>().UpdateSyncSoulToPlayer(-soulReduction, source: thisCard.transform);
        // AnimateAbilityExecute(adjCreature);

        // damage thisCard
        thisCard.GetComponent<CreatureStats>().UpdateCreatureDefense(amount: selfDamage, buff: false);

        //AnimateAbilityExecute(thisCard);
    }
}
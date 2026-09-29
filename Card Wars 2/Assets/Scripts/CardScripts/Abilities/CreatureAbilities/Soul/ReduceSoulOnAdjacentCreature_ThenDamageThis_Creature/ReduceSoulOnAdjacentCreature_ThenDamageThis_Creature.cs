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

[CreateAssetMenu(fileName = "ReduceSoulOnAdjacentCreature_ThenDamageThis_Creature",
    menuName = "Abilities/Creature/Soul/ReduceSoulOnAdjacentCreature_ThenDamageThis_Creature")]
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
        string leftOrRight = Side == AdjacentSide.Left ? "left" : "right";
        
        MiddleTile adj = thisCard.GetEitherAdjacentTile(leftOrRight);

        if (adj == null) return; // in a position where there is not right or left 

        // look at adjacent creature
        GameObject adjCreature = adj.logicalCreature;

        if (adjCreature == null)
        {
            return; // no creature on this tile
        }

        // buff that adjacent creature
        adjCreature.GetComponent<CreatureStats>().UpdateSyncSoulToPlayer(-soulReduction);
        AnimateAbilityExecute(adjCreature);

        // damage thisCard
        thisCard.GetComponent<CreatureStats>().UpdateCreatureDefense(amount: selfDamage, buff: false);

        AnimateAbilityExecute(thisCard);
    }
}
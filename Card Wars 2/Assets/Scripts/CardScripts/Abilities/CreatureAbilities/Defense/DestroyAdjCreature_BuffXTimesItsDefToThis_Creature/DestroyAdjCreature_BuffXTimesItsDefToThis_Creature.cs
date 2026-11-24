using System;
using System.Reflection;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardMovements;
using UnityEngine;
using PlayerStuff;
using Tiles;

[CreateAssetMenu(
    fileName = "DestroyAdjCreature_BuffXTimesItsDefToThis_Creature", 
    menuName = "Abilities/Creature/DestroyAdjCreature_BuffXTimesItsDefToThis_Creature")]
public class DestroyAdjCreature_BuffXTimesItsDefToThis_Creature : ActiveAbilitySO
{
    public float defMultiplier;
    
    public AdjacentSide Side;

    public enum AdjacentSide
    {
        Left,
        Right
    }
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        if (defMultiplier <= 0)
        {
            Debug.LogError($"{thisCard.name}: defMultiplier has to be greater than 0");
            return;
        }
        
        string leftOrRight = Side == AdjacentSide.Left ? "left" : "right";
        
        MiddleTile adj = thisCard.Ext_GetEitherAdjacentTile(leftOrRight);

        GameObject adjCreature = adj != null ? adj.logicalCreature : null;

        if (adjCreature == null) return;   // no adjacent tile, or no creature on it
        
        CreatureMovement adjMove = adjCreature.GetComponent<CreatureMovement>();

        if (adjMove.cardState == CardMovement.CardState.Discard)
        {
            // I had a thing where if factory was used on a card destroyed by this card,
            // it would repeatedly buff this card until it was destroyed after the fade. 
            
            // I had to reorder a bunch of stuff in the server discard function
            return;
        }

        // get adj creature
        CreatureStats adjCreatureStats = adjCreature.GetComponent<CreatureStats>();

        int def = adjCreatureStats.defense;
        
        Transform ajdTrans =  adjCreature.transform;
        
        // destroy adj creature
        CreatureMovement adjMovement = adjCreature.GetComponent<CreatureMovement>();
        
        adjMovement.ServerDiscard();
        
        // todo if creature was discarded:
        
        // buff
        int buff = (int)MathF.Ceiling(def * defMultiplier);
        
        CreatureStats thisCardStats = thisCard.GetComponent<CreatureStats>();
        
        thisCardStats.UpdateCreatureDefense(amount: buff, buff: true, applyMult: true, source: ajdTrans);
    }

    public override bool ValidateAbility(GameObject thisCard)
    {
        string leftOrRight = Side == AdjacentSide.Left ? "left" : "right";
        
        MiddleTile adj = thisCard.Ext_GetEitherAdjacentTile(leftOrRight);

        GameObject adjCreature = adj != null ? adj.logicalCreature : null;

        if (adjCreature == null)
        {
            string rejectMessage = $"No Creature to the {leftOrRight}";
            
            FloatCustomText_FromValidationReject(thisCard, rejectMessage);

            return false;
        }
        
        return true;
    }
}
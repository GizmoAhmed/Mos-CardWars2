using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using GameManagement;
using Tiles;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageOppLane_AtEndOfTurn_Rune",
    menuName = "Abilities/Runes/DamageOppLane_AtEndOfTurn_Rune")]
public class DamageOppLane_AtEndOfTurn_Rune : PassiveAbilitySO
{
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        RuneMovement rMove = thisCard.GetComponent<RuneMovement>();
        
        // wrath was placed, adjust mult instead of attacking
        if (eventData.eventType == AbilityEventType.CardPlacedOnTile)
        {
            // RuneMove.CreatureBoundTo won't work to get creature stats...
            // ...since rune not bound yet if broadcasted via placement listener
            
            // luckily placement event data passes a means to get tile, 
            // which can be used to get the creature at the tile the rune was placed on
            
            CreatureStats creatureStats = GetCreatureObjFromEventDataTile(eventData).GetComponent<CreatureStats>();
            
            // can't buff anymore
            creatureStats.canBeBuffed = false;
            
            return;
        } // if got through this block, then this ability was passed the end of turn exec, attack opp lane
        
        CreatureMovement cMove = rMove.creatureBoundTo.GetComponent<CreatureMovement>();

        Tile thisTile = cMove.GetLogicalTile();

        MiddleTile across = TileManager.Instance.GetAcrossTile(thisTile.row, 
                thisTile.column, 
                thisTile.serverPlayerSide) 
                as MiddleTile;

        CreatureStats cStats = rMove.creatureBoundTo.GetComponent<CreatureStats>();
        
        // strength of this wrath-ed creature
        int strength = cStats.strength;

        if (strength == 0) return; // no point 
        
        if (across.logicalCreature != null) // creature over there
        {
            CreatureStats oppCreature =  across.logicalCreature.GetComponent<CreatureStats>();
            
            // deal damage
            oppCreature.ChangeCreatureDefense(strength, buff:false);
        }
        else // empty lane 
        {
            // todo do something, for now, just add player drain or lost money or sum
        }
    }

    public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
    {
        RuneMovement rMove = thisCard.GetComponent<RuneMovement>();
        
        CreatureStats cStats = rMove.creatureBoundTo.GetComponent<CreatureStats>();
        
        cStats.canBeBuffed = true;
    }
}
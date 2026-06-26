using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using Extensions;
using UnityEngine;

[CreateAssetMenu(fileName = "AllowMultiFloop_BoundCreature_Rune",
    menuName = "Abilities/Runes/AllowMultiFloop_BoundCreature_Rune")]
public class AllowMultiFloop_BoundCreature_Rune : PassiveAbilitySO
{
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats creatureStats = GetCreatureObjFromEventDataTile(eventData).GetComponent<CreatureStats>();

        creatureStats.multiFloop = true;
    }
    
    public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats boundCreatureStats = thisCard.GetCreatureStats_FromBoundRune_Ext();

        if (boundCreatureStats == null)
        {
            Debug.LogError($"Creature bound by this rune ({thisCard}) was found null");
            return;
        }

        // reset
        boundCreatureStats.multiFloop = false;
    }
}

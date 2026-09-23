using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using UnityEngine;

[CreateAssetMenu(fileName = "ReduceBurnCost_OfCreatureOnTile_Building",
    menuName = "Abilities/Building/ReduceBurnCost_OfCreatureOnTile_Building")]
public class ReduceBurnCost_OfCreatureOnTile_Building : PassiveAbilitySO
{
    public int burnReduction;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats creatureStats = GetCreature_FromTileInEventData(thisCard, eventData);

        if (creatureStats != null)
        {
            if (creatureStats.burnCost > 0) // don't be negative
            {
                creatureStats.burnCost -= burnReduction;
                AnimateAbilityExecute(thisCard);
            }
        }
    }

    public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats creatureStats = GetCreature_FromTileInEventData(thisCard, eventData);

        if (creatureStats != null)
        {
            creatureStats.burnCost += burnReduction;
        }
    }
}
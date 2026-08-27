using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using UnityEngine;

[CreateAssetMenu(fileName = "ReduceBurnCost_OfPlayersCreaturesPlaced_Charm",
    menuName = "Abilities/Charm/ReduceBurnCost_OfPlayersCreaturesPlaced_Charm")]
public class ReduceBurnCost_OfPlayersCreaturesPlaced_Charm : PassiveAbilitySO
{
    public int burnReduction;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        Debug.Log($"<<< {name} on {thisCard.name} execution detected >>>");

        if (eventData.eventType == AbilityEventType.CardPlacedOnTile)
        {
            ReduceBurnOfAllCreatures(card: thisCard, burnReduction);
        }
        else if (eventData.eventType == AbilityEventType.AnyCreaturePlaced)
        {
            bool yourCard = eventData.target.Ext_IsCardOwnedByThisPlayer(player: thisCard.Ext_GetOwningPlayerStats());

            if (yourCard)
            {
                CreatureStats placedCreature = eventData.target.GetComponent<CreatureStats>();

                placedCreature.burnCost -= burnReduction;
            }
        }
    }

    public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
    {
        ReduceBurnOfAllCreatures(card: thisCard, -burnReduction); // negative, since increasing burn cost
    }

    private void ReduceBurnOfAllCreatures(GameObject card, int amount)
    {
        var creatures = card.Ext_GetAllActiveCreaturesForThisPlayer();

        foreach (var creature in creatures)
        {
            CreatureStats stats = creature.GetComponent<CreatureStats>();

            if (amount > 0) // reducing
            {
                if (stats.burnCost > 0) // if subtracting (amount > 0), don't let burn be negative
                {
                    stats.burnCost -= amount;
                }
            }
            else // adding back from undo
            {
                stats.burnCost -= amount;
            }
        }
    }
}
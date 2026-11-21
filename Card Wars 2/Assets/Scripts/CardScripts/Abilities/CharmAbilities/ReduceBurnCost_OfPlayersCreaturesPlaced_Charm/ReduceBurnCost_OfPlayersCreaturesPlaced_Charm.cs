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
            AdjustBurnOfAllCreatures(card: thisCard, burnReduction, increase: false);
            // AnimateAbilityExecute(thisCard);
        }
        else if (eventData.eventType == AbilityEventType.AnyCreaturePlaced)
        {
            bool owned = thisCard.Ext_IsSameOwner(eventData.target);   

            if (owned)
            {
                CreatureStats placedCreature = eventData.target.GetComponent<CreatureStats>();

                placedCreature.UpdateBurnCost(burnReduction, increase: false, source: thisCard.transform);
                AnimateAbilityExecute(thisCard);
            }
        }
    }

    public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
    {
        AdjustBurnOfAllCreatures(card: thisCard, burnReduction , increase: true); // negative, since increasing burn cost
    }

    private void AdjustBurnOfAllCreatures(GameObject card, int amount, bool increase)
    {
        var creatures = card.Ext_GetAllActiveCreaturesForThisPlayer();

        foreach (var creature in creatures)
        {
            CreatureStats stats = creature.GetComponent<CreatureStats>();

            stats.UpdateBurnCost(amount, increase: increase, source: card.transform);

        }
    }
}
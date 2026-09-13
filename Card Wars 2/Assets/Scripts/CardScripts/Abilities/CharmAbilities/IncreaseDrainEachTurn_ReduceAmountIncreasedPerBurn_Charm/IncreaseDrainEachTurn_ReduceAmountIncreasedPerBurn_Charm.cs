using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts;
using CardScripts.Abilities;
using CardScripts.CardMovements;
using Extensions;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(fileName = "IncreaseDrainEachTurn_ReduceAmountIncreasedPerBurn_Charm",
    menuName = "Abilities/Charm/IncreaseDrainEachTurn_ReduceAmountIncreasedPerBurn_Charm")]
public class IncreaseDrainEachTurn_ReduceAmountIncreasedPerBurn_Charm : PassiveAbilitySO
{
    // how much buff to start with
    public int startingDrainBuffAmount;
    private const string DrainBuffKey = "CurrentDrainBuffAmount";

    public int buffReductionPerBurn;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        Debug.Log($"{name} on {thisCard.name}: <color=magenta>{eventData.eventType}</color>");

        CardRuntimeData data = thisCard.GetComponent<CardRuntimeData>();

        switch (eventData.eventType)
        {
            case AbilityEventType.CardPlacedOnTile:

                // set CardRuntimeData custom data 
                data.customInts[DrainBuffKey] = startingDrainBuffAmount;
                break;

            case AbilityEventType.AnyCardBurned:

                if (eventData.target == thisCard)// burning itself
                {
                    return;
                }

                if (!CardRuntimeData_ContainsDrainKey(data)) // check if the dict entry was made
                {
                    return;
                }

                GameObject burnedCard = eventData.target;

                bool owned = thisCard.Ext_IsSameOwner(burnedCard);

                // if you don't own the burned card, return
                if (!owned)
                {
                    return;
                }

                // reduce buff amount
                data.customInts[DrainBuffKey] -= buffReductionPerBurn;

                // if below zero, discard
                if (data.customInts[DrainBuffKey] < 0)
                {
                    thisCard.GetComponent<CharmMovement>().ServerDiscard();
                }

                break;

            case AbilityEventType.AnyTurnEnd:
                
                if (!CardRuntimeData_ContainsDrainKey(data)) // check if the dict entry was made
                {
                    return;
                }

                // get player
                PlayerStats player = thisCard.Ext_GetOwningPlayerStats();
                
                // buff drain
                player.drain += data.customInts[DrainBuffKey];
                
                break;

            default:
                Debug.LogError($"Unsupported event type passed to {name} on {thisCard.name}");
                break;
        }
    }

    private bool CardRuntimeData_ContainsDrainKey(CardRuntimeData cardRuntimeData)
    {
        if (cardRuntimeData.customInts.ContainsKey(DrainBuffKey)) // check if the dict entry was made
        {
            return true;
        }

        Debug.LogError(
            $"<color=orange>{name}</color>: Dictionary entry for key {DrainBuffKey} was not found.");
        return false;
    }
}
using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardMovements;
using CardScripts.CardStats_Folder;
using UnityEngine;

[CreateAssetMenu(fileName = "ReduceSoul_OnThisCard_OnAnyBurn_Charm", menuName = "Abilities/Charm/ReduceSoul_OnThisCard_OnAnyBurn_Charm")]
public class ReduceSoul_OnThisCard_OnAnyBurn_Charm : PassiveAbilitySO
{
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        Debug.Log("I heard a card was burned around here...");

        if (eventData.targetCard == thisCard) return; // if executed on itself, don't
        
        GameObject burnedCard = eventData.targetCard;

        int thisSide = thisCard.GetComponent<CardMovement>().logicalPlayerSide;
        int thatSide = burnedCard.GetComponent<CardMovement>().logicalPlayerSide;

        if (thisSide != thatSide) return; // only count owned cards

        // has to be on the field
        if (burnedCard.GetComponent<CardMovement>().cardState 
            != CardMovement.CardState.Field)
        {
            return;
        }
        
        // subtract soul use
        thisCard.GetComponent<CardStats>().soulUse -= burnedCard.GetComponent<CardStats>().soulUse;
    }

    public void OnValidate()
    {
        if (isExecutableOnPlaced)
        {
            Debug.LogError($"{name} can't be executable on place");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardMovements;
using CardScripts.CardStats_Folder;
using Extensions;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(fileName = "ReduceSoul_OnThisCard_OnAnyBurn_Charm",
    menuName = "Abilities/Charm/ReduceSoul_OnThisCard_OnAnyBurn_Charm")]
public class ReduceSoul_OnThisCard_OnAnyBurn_Charm : PassiveAbilitySO
{
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        if (eventData.target == thisCard) return; // if executed on itself, don't

        GameObject burnedCard = eventData.target;

        bool isOwned = thisCard.Ext_IsSameOwner(burnedCard);
        // bool isOwned = burnedCard.Ext_IsCardOwnedByThisPlayer(thisCard.Ext_GetOwningPlayerStats());

        if (!isOwned) return; // not your card

        // has to be on the field, otherwise return
        if (!burnedCard.Ext_isCardOnField()) return;

        int burnedCardSoul = burnedCard.GetComponent<CardStats>().soulUse;

        // reduce soul use
        thisCard.GetComponent<CardStats>().UpdateSyncSoulToPlayer(-burnedCardSoul);
        AnimateAbility(thisCard);
    }

    public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
    {
        // does nothing
    }

    public void OnValidate()
    {
        if (isExecutableOnPlaced)
        {
            Debug.LogError($"{name} can't be executable on place");
        }
    }
}
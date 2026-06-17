using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using Extensions;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(fileName = "Redraw_LastDiscardedCard_Floop_Creature",
    menuName = "Abilities/Creature/Draw/Redraw_LastDiscardedCard_Floop_Creature")]
public class Redraw_LastDiscardedCard_Floop_Creature : ActiveAbilitySO
{
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        Debug.Log($"<color=cyan>{name}</color> active...");

        PlayerCardTracker tracker = thisCard.GetOwningCardTracker_Ext();
        
        GameObject lastCard = tracker.Server_GetLastDiscard();
            
        if (lastCard == null)
        {
            Debug.LogWarning($"<color=cyan>{name} on {thisCard.name} is ineffective</color>: No discarded cards on {tracker.gameObject.name}");
            return;
        }
            
        RedrawCard(lastCard);
    }
}
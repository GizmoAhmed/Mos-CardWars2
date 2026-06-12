using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(fileName = "Redraw_LastDiscardedCard_Floop_Creature",
    menuName = "Abilities/Creature/Draw/Redraw_LastDiscardedCard_Floop_Creature")]
public class Redraw_LastDiscardedCard_Floop_Creature : ActiveAbilitySO
{
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        Debug.Log($"<color=cyan>{name}</color> active...");

        Player player = thisCard.GetComponent<CreatureMovement>().thisCardOwnerPlayerStats.GetComponent<Player>();

        if (player.cardTracker == null)
        {
            Debug.LogWarning("cardTracker is null on {player}");
        }
        else
        {
            GameObject lastCard = player.cardTracker.Server_GetLastDiscard();
            
            if (lastCard == null)
            {
                Debug.LogWarning($"<color=cyan>{name} on {thisCard.name} is ineffective</color>: No discarded cards on {player}");
                return;
            }
            
            RedrawCard(lastCard);
        }
    }
}
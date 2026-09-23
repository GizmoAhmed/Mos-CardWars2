using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardMovements;
using CardScripts.CardStatss;
using Extensions;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockOppSoulUpgrade_UntilOppBurnsCreatureOfXScore_Charm", menuName = "Abilities/Charm/BlockOppSoulUpgrade_UntilOppBurnsCreatureOfXScore_Charm")]
public class BlockOppSoulUpgrade_UntilOppBurnsCreatureOfXScore_Charm : PassiveAbilitySO
{
    [Tooltip("Burned creature needs at least this score to remove this charm")]
    public int scoreNeeded;
    
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        PlayerStats opponent = thisCard.Ext_GetOwningPlayerStats().Ext_GetOpponentPlayerStats();
        
        if (eventData.eventType == AbilityEventType.CardPlacedOnTile) // charm placed
        {
            // block soul upgrades
            opponent.canUpgradeSoul += 1;
            AnimateAbilityExecute(thisCard);
        }
        else if (eventData.eventType == AbilityEventType.AnyCreatureBurned)
        {
            // check if opp card
            GameObject burnedCard = eventData.target.gameObject;

            if (!burnedCard.Ext_IsCardOwnedByThisPlayer(opponent)) // not owned by opponent
            {
                return;
            }

            // check if on field

            if (!burnedCard.Ext_isCardOnField()) // not on the field
            {
                return;
            }

            // get score
            // if score exceeds required, discard this charm

            CreatureStats stats = burnedCard.GetComponent<CreatureStats>();

            if (stats.score >= scoreNeeded)
            {
                CharmMovement thisCharmMove = thisCard.GetComponent<CharmMovement>();
                thisCharmMove.ServerDiscard();
            }
        }
        else
        {
            Debug.LogError($"{name} on {thisCard.name} received an unusable event type: <color=red>{eventData.eventType}</color>");
        }
    }

    public override void UndoExecution(GameObject thisCard, AbilityEventData eventData)
    {
        // get opp
        PlayerStats opponent = thisCard.Ext_GetOwningPlayerStats().Ext_GetOpponentPlayerStats();
            
        // block soul upgrades
        opponent.canUpgradeSoul -= 1;
    }
}

using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardMovements;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(
    fileName = "DiscardMostRecentlyDrawnCardFromOppsHand_Creature", 
    menuName = "Abilities/Creature/Draw/DiscardMostRecentlyDrawnCardFromOppsHand_Creature")]
public class DiscardMostRecentlyDrawnCardFromOppsHand_Creature : ActiveAbilitySO
{
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        PlayerCardTracker oppCardTracker = thisCard.Ext_GetOpponentCardTracker();

        List<GameObject> oppHand = oppCardTracker.GetThisPlayersHand();

        if (oppHand.Count == 0) return;
        
        GameObject lastDrawnOppCard = oppHand[^1]; // from end
        
        CardMovement cardMovement = lastDrawnOppCard.GetComponent<CardMovement>();
        
        cardMovement.ServerDiscard();
    }
}
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardMovements;
using UnityEngine;
using PlayerStuff;
using Tiles;

[CreateAssetMenu(
    fileName = "DestroyXCardsInHand_IncreaseFreeOffering_Creature", 
    menuName = "Abilities/Creature/Draw/DestroyXCardsInHand_IncreaseFreeOffering_Creature")]
public class DestroyXCardsInHand_IncreaseFreeOffering_Creature : ActiveAbilitySO
{
    public int offeringsGiven = 1;
    public int minCardsToDestroy;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        if (thisCard.Ext_GetPlayerHandCount() < minCardsToDestroy) return ;// not enough cards

        List<GameObject> playersHand = thisCard.Ext_GetThisPlayerHand();

        // discard cards from hand randomly
        foreach (GameObject card in playersHand.PickRandomItemsFromList(count: minCardsToDestroy))
        {
            if (card.TryGetComponent(out CardMovement move))
            {
                move.ServerDiscard();
            }
        }

        PlayerStats playerStats = thisCard.Ext_GetOwningPlayerStats();
            
        playerStats.UpdateFreeCardsOffered(amount: offeringsGiven, increase: true);
    }
    
    [Header("Rejection Message")]
    [SerializeField] [TextArea] private string rejectMessage;

    public override bool ValidateAbility(GameObject thisCard)
    {
        if (!base.ValidateAbility(thisCard)) 
        { 
            return false; // base checks fail
        }

        if (thisCard.Ext_GetPlayerHandCount() < minCardsToDestroy) // not enough cards
        {
            FloatCustomText_FromValidationReject(thisCard, rejectMessage);
            return false;
        }

        return true;
    }
}
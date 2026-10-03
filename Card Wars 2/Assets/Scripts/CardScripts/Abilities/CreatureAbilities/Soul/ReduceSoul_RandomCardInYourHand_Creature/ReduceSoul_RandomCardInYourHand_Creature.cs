using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using CardScripts.CardStats_Folder;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(
    fileName = "ReduceSoul_RandomCardInYourHand_Creature", 
    menuName = "Abilities/Creature/ReduceSoul_RandomCardInYourHand_Creature")]
public class ReduceSoul_RandomCardInYourHand_Creature : ActiveAbilitySO
{
    public int soulReduction;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        // get hand
        PlayerCardTracker cardTracker = thisCard.Ext_GetOwningCardTracker();

        List<GameObject> hand = cardTracker.GetThisPlayerSoulCardsInHand();

        // pick random card, that uses magic

        CardStats randomPicked = hand.GetRandomFromList().GetComponent<CardStats>();
        
        randomPicked.UpdateSyncSoulToPlayer(-soulReduction);
        AnimateAbilityExecute(thisCard);
        AnimateAbilityExecute(randomPicked.gameObject);
    }
}
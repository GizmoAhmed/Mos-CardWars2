using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStats_Folder;
using CardScripts.CardStatss;
using Extensions;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(
    fileName = "DoubleSoulUse_OnRandomOppCard_Creature", 
    menuName = "Abilities/Creature/DoubleSoulUse_OnRandomOppCard_Creature")]
public class DoubleSoulUse_OnRandomOppCard_Creature : ActiveAbilitySO
{
    // TODO: Add your ability parameters here
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        PlayerCardTracker oppCardTracker = thisCard.Ext_GetOpponentCardTracker();

        List<CardStats> oppsActiveCards = oppCardTracker.Server_GetThisPlayerAllActiveFieldCards();

        if (oppsActiveCards == null || oppsActiveCards.Count == 0)
        {
            return;
        }

        CardStats randomPicked = oppsActiveCards.GetRandomFromList();
        
        randomPicked.UpdateSyncSoulToPlayer(randomPicked.soulUse); // double
        
        AnimateAbilityExecute(thisCard);
        AnimateAbilityExecute(randomPicked.gameObject);
    }
}
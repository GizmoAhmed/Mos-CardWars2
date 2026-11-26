using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;
using PlayerStuff;
using Tiles;

[CreateAssetMenu(
    fileName = "GetXFreeDrawOfferingNextTurn_Creature", 
    menuName = "Abilities/Creature/Draw/GetXFreeDrawOfferingThisTurn_Creature")]
public class GetXFreeDrawOfferingThisTurn_Creature : ActiveAbilitySO
{
    public int tempOffering;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        PlayerStats playerStats = thisCard.Ext_GetOwningPlayerStats(); 
        playerStats.UpdateFreeCardsOffered(amount: tempOffering, increase: true);
        
        PlayerCardTracker thisCardTracker = thisCard.Ext_GetOwningCardTracker();
        thisCardTracker.tempFreeOffering_ResetOnSwapPhase += tempOffering; // update this so it uses it to reset the actual player stat later
        
        AnimateAbilityExecute(thisCard);
    }
}
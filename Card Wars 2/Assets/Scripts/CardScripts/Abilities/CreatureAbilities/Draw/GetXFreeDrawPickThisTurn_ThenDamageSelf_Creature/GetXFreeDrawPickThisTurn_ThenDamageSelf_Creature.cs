using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;
using PlayerStuff;
using Tiles;

[CreateAssetMenu(
    fileName = "GetXFreeDrawPickThisTurn_Creature", 
    menuName = "Abilities/Creature/GetXFreeDrawPickThisTurn_Creature")]
public class GetXFreeDrawPickThisTurn_ThenDamageSelf_Creature : ActiveAbilitySO
{
    public int tempPicks;
    public int selfDamage;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        PlayerStats playerStats = thisCard.Ext_GetOwningPlayerStats();
        playerStats.UpdateFreeCardPicks(amount: tempPicks, increase: true);
        
        PlayerCardTracker thisCardTracker = thisCard.Ext_GetOwningCardTracker();
        thisCardTracker.tempFreePicks_ResetOnSwapPhase += tempPicks; // update this so it uses it to reset the actual player stat later
        
        CreatureStats stats = thisCard.GetComponent<CreatureStats>();
        
        stats.UpdateCreatureDefense(amount: selfDamage, buff: false);
        
        AnimateAbilityExecute(thisCard);
    }
}
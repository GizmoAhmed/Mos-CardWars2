using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;

[CreateAssetMenu(
    fileName = "StrengthenPerAbilityCost_RaiseAbilityCost_Creature", 
    menuName = "Abilities/Creature/StrengthenPerAbilityCost_RaiseAbilityCost_Creature")]
public class StrengthenPerAbilityCost_RaiseAbilityCost_Creature : ActiveAbilitySO
{
    public int strengthPerCost;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        // get ability cost
        CreatureStats stats = thisCard.GetComponent<CreatureStats>();
        int cost = stats.abilityCost;

        if (cost < 0) // negative
        {
            stats.UpdateCreatureStrength(cost*strengthPerCost, buff: false); // nerf
        }
        else
        {
            stats.UpdateCreatureStrength(cost*strengthPerCost, buff: true); // buff
        }

        // raise ability cost, todo preferably offset with an invoke or something
        stats.abilityCost++;
        
        AnimateAbilityExecute(thisCard);
    }
}
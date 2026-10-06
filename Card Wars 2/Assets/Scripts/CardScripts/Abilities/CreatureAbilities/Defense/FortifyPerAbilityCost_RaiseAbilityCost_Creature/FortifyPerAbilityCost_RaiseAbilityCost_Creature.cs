using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;

[CreateAssetMenu(
    fileName = "FortifyPerAbilityCost_RaiseAbilityCost_Creature", 
    menuName = "Abilities/Creature/FortifyPerAbilityCost_RaiseAbilityCost_Creature")]
public class FortifyPerAbilityCost_RaiseAbilityCost_Creature : ActiveAbilitySO
{
    public int fortifyPerCost;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        // get ability cost
        CreatureStats stats = thisCard.GetComponent<CreatureStats>();
        int cost = stats.abilityCost;

        if (cost < 0) // negative
        {
            stats.UpdateCreatureDefense(cost*fortifyPerCost, buff: false); // nerf
        }
        else
        {
            stats.UpdateCreatureDefense(cost*fortifyPerCost, buff: true); // buff
        }

        // raise ability cost, todo preferably offset with an invoke or something
        stats.abilityCost++;
        
        AnimateAbilityExecute(thisCard);
    }
}
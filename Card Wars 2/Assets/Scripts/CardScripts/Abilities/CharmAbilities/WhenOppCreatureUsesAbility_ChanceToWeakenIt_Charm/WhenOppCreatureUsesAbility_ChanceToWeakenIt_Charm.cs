using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;
using PlayerStuff;
using Tiles;

[CreateAssetMenu(
    fileName = "WhenOppCreatureUsesAbility_ChanceToWeakenIt_Charm", 
    menuName = "Abilities/Charm/WhenOppCreatureUsesAbility_ChanceToWeakenIt_Charm")]
public class WhenOppCreatureUsesAbility_ChanceToWeakenIt_Charm : PassiveAbilitySO
{
    [Tooltip("Chance to weaken when opp uses creature ability")]
    [Range(1, 100)] public int chanceToHit;

    public int weaken;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        GameObject creatureActivated = eventData.target;
        
        bool yours = thisCard.Ext_IsSameOwner(creatureActivated);

        if (yours) return; // pass if yours

        if (RollChance(chanceToHit, thisCard)) // if hits
        {
            CreatureStats stats = creatureActivated.GetComponent<CreatureStats>();
            
            stats.UpdateCreatureStrength(amount: weaken, buff: false, applyMult:true);
            AnimateAbilityExecute(thisCard);
        }
    }
}
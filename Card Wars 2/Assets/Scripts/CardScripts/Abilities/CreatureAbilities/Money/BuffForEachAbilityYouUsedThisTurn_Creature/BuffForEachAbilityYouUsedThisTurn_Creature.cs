using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using UnityEngine;
using PlayerStuff;
using Tiles;

[CreateAssetMenu(
    fileName = "BuffForEachAbilityYouUsedThisTurn_Creature", 
    menuName = "Abilities/Creature/Money/BuffForEachAbilityYouUsedThisTurn_Creature")]
public class BuffForEachAbilityYouUsedThisTurn_Creature : ActiveAbilitySO
{
    public int strengthenPerAbilityUsedThisTurn;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        PlayerCardTracker playerCardTracker = thisCard.Ext_GetOwningCardTracker();

        int floops = playerCardTracker.creatureActivationsThisTurn;

        if (floops <= 0) return; // no point
        
        int str = floops * strengthenPerAbilityUsedThisTurn;
        
        CreatureStats creature =  thisCard.GetComponent<CreatureStats>();

        creature.UpdateCreatureStrength(amount: str, buff: true, applyMult: true);
        
        AnimateAbilityExecute(thisCard);
    }
}
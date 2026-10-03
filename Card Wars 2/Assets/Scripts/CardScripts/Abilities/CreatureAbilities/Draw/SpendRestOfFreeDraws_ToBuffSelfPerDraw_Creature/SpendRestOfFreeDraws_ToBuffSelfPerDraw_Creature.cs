using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(
    fileName = "SpendRestOfFreeDraws_ToBuffSelfPerDraw_Creature", 
    menuName = "Abilities/Creature/SpendRestOfFreeDraws_ToBuffSelfPerDraw_Creature")]
public class SpendRestOfFreeDraws_ToBuffSelfPerDraw_Creature : ActiveAbilitySO
{
    public int strengthPerDraw;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        PlayerStats player = thisCard.Ext_GetOwningPlayerStats();

        int freeDrawsLeft = player.freeDrawsLeft;

        if (freeDrawsLeft == 0)
        {
            return; // no point
        }

        thisCard.GetComponent<CreatureStats>().
            UpdateCreatureStrength(amount: strengthPerDraw * freeDrawsLeft, buff: true);
        
        Debug.Log($"Spent <color=pink>{freeDrawsLeft}</color> to strengthen by {strengthPerDraw * freeDrawsLeft}");
        
        player.freeDrawsLeft = 0; // set to zero\
        
        AnimateAbilityExecute(thisCard);
    }
}
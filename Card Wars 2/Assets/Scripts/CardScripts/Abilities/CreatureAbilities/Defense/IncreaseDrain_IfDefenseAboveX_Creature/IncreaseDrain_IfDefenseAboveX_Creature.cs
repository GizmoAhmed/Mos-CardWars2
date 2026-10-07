using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(
    fileName = "IncreaseDrain_IfDefenseAboveX_Creature", 
    menuName = "Abilities/Creature/Defense/IncreaseDrain_IfDefenseAboveX_Creature")]
public class IncreaseDrain_IfDefenseAboveX_Creature : ActiveAbilitySO
{
    public int defenseThreshold;
    public int drainIncrease;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        CreatureStats creature = thisCard.GetComponent<CreatureStats>();
        
        int def = creature.defense;

        // didn't meet threshold
        if (def <= defenseThreshold) return;

        PlayerStats player = thisCard.Ext_GetOwningPlayerStats();
        
        player.drain += drainIncrease;
        AnimateAbilityExecute(thisCard);
    }
}
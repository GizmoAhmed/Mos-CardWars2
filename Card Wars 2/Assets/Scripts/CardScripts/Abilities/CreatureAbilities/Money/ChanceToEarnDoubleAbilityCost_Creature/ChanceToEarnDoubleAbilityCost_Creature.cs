using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(
    fileName = "ChanceToEarnDoubleAbilityCost_Creature",
    menuName = "Abilities/Creature/ChanceToEarnDoubleAbilityCost_Creature")]
public class ChanceToEarnDoubleAbilityCost_Creature : ActiveAbilitySO
{
    [Tooltip("Chance to get return")] [Range(1, 100)]
    public int hitChance;

    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        if (RollChance(hitChance))
        {
            // get player
            PlayerStats player = thisCard.Ext_GetOwningPlayerStats();

            CreatureStats creature = thisCard.GetComponent<CreatureStats>();
            int cost = creature.abilityCost;

            player.shards += (cost * 2);
            
            Debug.Log($"<color=yellow>{this} on {thisCard.name}</color> just <color=green>Hit</color>, doubling return...");
        }
        else
        {
            Debug.Log($"<color=yellow>{this} on {thisCard.name}</color> just <color=red>missed</color>, no return...");
        }

        AnimateAbilityExecute(thisCard);
    }
}
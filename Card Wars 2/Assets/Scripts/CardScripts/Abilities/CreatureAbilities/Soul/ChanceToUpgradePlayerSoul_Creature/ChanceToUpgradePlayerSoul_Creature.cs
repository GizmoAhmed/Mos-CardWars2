using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(
    fileName = "ChanceToUpgradePlayerSoul_Creature", 
    menuName = "Abilities/Creature/Soul/ChanceToUpgradePlayerSoul_Creature")]
public class ChanceToUpgradePlayerSoul_Creature : ActiveAbilitySO
{
    [Tooltip("Chance to Hit")]
    [Range(1, 100)] public int chanceToHit;

    public int soulUpgradeAmount;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        if (RollChance(chanceToHit)) // hits
        {
            PlayerStats player = thisCard.Ext_GetOwningPlayerStats();

            player.UpdatePlayerMaxSoul_ViaUpgrade(
                increase: true, 
                amount: soulUpgradeAmount,
                ignoreAnySoulUpgradeBlock: false);
            
            AnimateAbilityExecute(thisCard);
            
            Debug.Log($"{this.name} on {thisCard.name} just <color=green>hit</color>");
        }
        else
        {
            Debug.Log($"{this.name} on {thisCard.name} just <color=red>missed</color>");

        }
    }
}
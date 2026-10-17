using AbilityEvents;
using CardScripts.Abilities;
using CardScripts.CardStatss;
using Extensions;
using CardScripts.Abilities.AbilityClasses;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(
    fileName = "ChanceUpgradeSoul_EndOfTurn_IfNoCreatureActivatedThisTurn_Charm", 
    menuName = "Abilities/Charm/ChanceUpgradeSoul_EndOfTurn_IfNoCreatureActivatedThisTurn_Charm")]
public class ChanceUpgradeSoul_EndOfTurn_IfNoCreatureActivatedThisTurn_Charm : PassiveAbilitySO
{
    public int soulUpgradeAmount;
    
    [Tooltip("Chance to Hit")]
    [Range(1, 100)] public int chanceToHit;
        
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        PlayerCardTracker tracker = thisCard.Ext_GetOwningCardTracker();

        int floopsThisTurn = tracker.creatureActivationsThisTurn;

        if (floopsThisTurn == 0)
        {
            if (RollChance(chanceToHit, thisCard)) // hits
            {
                PlayerStats player = thisCard.Ext_GetOwningPlayerStats();

                player.UpdatePlayerMaxSoul_ViaUpgrade(
                    increase: true, 
                    amount: soulUpgradeAmount,
                    ignoreAnySoulUpgradeBlock: false);
            
                AnimateAbilityExecute(thisCard);
            
                Debug.Log($"{this.name} on {thisCard.name} just <color=green>hit</color>");
            }
        }
    }
}
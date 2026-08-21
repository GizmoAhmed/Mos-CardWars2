using System.Collections;
using System.Collections.Generic;
using AbilityEvents;
using CardScripts.Abilities;
using Extensions;
using PlayerStuff;
using UnityEngine;

[CreateAssetMenu(fileName = "IncreaseDrain_EachTurn_IfPlayerLowMoney_Charm", menuName = "Abilities/Charm/IncreaseDrain_EachTurn_IfPlayerLowMoney_Charm")]
public class IncreaseDrain_EachTurn_IfPlayerLowMoney_Charm : PassiveAbilitySO
{
    [Tooltip("If player has less money that this, activate the ability each turn")]
    public int moneyThreshold;
    
    [Tooltip("Amount of drain to gain upon charm ability execution")]
    public int drainAmount;
    
    public override void ExecuteAbility(GameObject thisCard, AbilityEventData eventData)
    {
        Debug.Log($"{name} on {thisCard.name}: End-of-turn-detected.exe");
        
        PlayerStats thisPlayer = thisCard.Ext_GetOwningPlayerStats();

        if (moneyThreshold <= 0)
        {
            Debug.LogWarning($"{name} on {thisCard.name} has a <color=orange>money threshold that's less than or equal to zero</color>, which doesn't make sense for this ability. Make sure to set a valid number in the inspector");
        }

        if (thisPlayer.shards < moneyThreshold)
        {
            thisPlayer.drain += drainAmount;
        }
    }
}
